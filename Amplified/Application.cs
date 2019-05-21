using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Amplified
{
    static class AmplifiedApp
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            m_instance = new AmplifiedAppInstance();

        }
        private static AmplifiedAppInstance m_instance = null;

        public static AmplifiedAppInstance Instance { get => m_instance; private set => m_instance = value; }
    }

    public class GenericEventArgs<TEventDataType> : EventArgs
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public TEventDataType Data { get; set; }
    }

    public class SingleInstance : IDisposable
    {
        private readonly bool ownsMutex;
        private Mutex mutex;
        private Guid identifier;

        /// <summary>
        /// Occurs when [arguments received].
        /// </summary>
        public event EventHandler<GenericEventArgs<string>> ArgumentsReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleInstance"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public SingleInstance(Guid id)
        {
            this.identifier = id;
            mutex = new Mutex(true, identifier.ToString(), out ownsMutex);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is first instance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is first instance; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstInstance
        {
            get
            {
                return ownsMutex;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (mutex != null && ownsMutex)
            {
                mutex.ReleaseMutex();
                mutex = null;
            }
        }

        /// <summary>
        /// Passes the arguments to first instance.
        /// </summary>
        /// <param name="argument">The argument.</param>
        public void PassArgumentsToFirstInstance(string argument)
        {
            using (var client = new NamedPipeClientStream(identifier.ToString()))
            using (var writer = new StreamWriter(client))
            {
                client.Connect(200);
                writer.WriteLine(argument);
            }
        }

        /// <summary>
        /// Listens for arguments from successive instances.
        /// </summary>
        public void ListenForArgumentsFromSuccessiveInstances()
        {
            Task.Factory.StartNew(() =>
            {

                using (var server = new NamedPipeServerStream(identifier.ToString()))
                using (var reader = new StreamReader(server))
                {
                    while (true)
                    {
                        server.WaitForConnection();

                        var argument = string.Empty;
                        while (server.IsConnected)
                        {
                            argument += reader.ReadLine();
                        }

                        CallOnArgumentsReceived(argument);
                        server.Disconnect();
                    }
                }
            });
        }

        /// <summary>
        /// Calls the on arguments received.
        /// </summary>
        /// <param name="state">The state.</param>
        public void CallOnArgumentsReceived(object state)
        {
            if (ArgumentsReceived != null)
            {
                if (state == null)
                {
                    state = string.Empty;
                }

                ArgumentsReceived(this, new GenericEventArgs<string>() { Data = state.ToString() });
            }
        }
    }


    public class AmplifiedAppInstance : Application
    {
        private static readonly SingleInstance SInstance = new SingleInstance(new Guid("24D910A1-1F03-44BA-85A0-BE7BC2655FFE"));

        public AmpAppMainGUIViewModel ViewModel
        {
            get;
            set;
        }

        public AmpAppMainGUI View
        {
            get;
            set;
        }

        public AmplifiedAppInstance()
        {
            View = new AmpAppMainGUI();
            ViewModel = new AmpAppMainGUIViewModel();
            View.DataContext = ViewModel;
            Run(View);
        }

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            if (SInstance.IsFirstInstance)
            {
                SInstance.ArgumentsReceived += ReceivedArg;
                SInstance.ListenForArgumentsFromSuccessiveInstances();
            }
            else
            {
                // if there is an argument available, fire it
                if (e.Args.Length > 0)
                {
                    SInstance.PassArgumentsToFirstInstance(e.Args[0]);
                }

                Environment.Exit(0);
            }
        }

        public void ReceivedArg(object sender, GenericEventArgs<string> e)
        {
            // Inform app of new arguments
            if (ViewModel != null)
            {
                ViewModel.OnArgReceived(e.Data as string);
            }
        }
    }

}