using System;
using System.IO;
using TurtleCoinAPI;

namespace API_Example
{
    /*
     * This is an example program that will create an API session, open a daemon connection,
     * load a wallet using that connection, sync both, and then display the wallet's total balances
     */
    class Program
    {
        // Declare an API session
        public TurtleCoin _session;

        // Print daemon log to console
        public void DaemonLog(object sender, TurtleCoinLogEventArgs e)
        {
            Console.WriteLine("Daemon:\t{0}", e.Message);
        }

        // Print wallet log to console
        public void WalletLog(object sender, TurtleCoinLogEventArgs e)
        {
            Console.WriteLine("Wallet:\t{0}", e.Message);
        }

        // Print errors to console
        public void Error(object sender, TurtleCoinErrorEventArgs e)
        {
            Console.WriteLine("Error:\t{0}", e.ErrorCode);
        }

        // Daemon update event
        public void OnDaemonUpdate(object sender, EventArgs e)
        {
            // Daemon must read as synced to send requests
            if (!(sender as Daemon).Synced)
                Console.WriteLine("Daemon:\tSyncing - {0} / {1}", (sender as Daemon).Height, (sender as Daemon).NetworkHeight);
        }

        // Wallet update event
        public void OnWalletUpdate(object sender, EventArgs e)
        {
            if (!(sender as Wallet).Synced)
                Console.WriteLine("Wallet:\tSyncing - {0} / {1}", (sender as Wallet).BlockCount, (sender as Wallet).KnownBlockCount);
            else
                Console.WriteLine("Available Balance: {0}, Locked Amount: {1}", (sender as Wallet).AvailableBalance, (sender as Wallet).LockedAmount);
        }

        // Other events
        public void OnDaemonConnect(object sender, EventArgs e) { }
        public void OnDaemonDisconnect(object sender, EventArgs e) { }
        public void OnWalletConnect(object sender, EventArgs e) { }
        public void OnWalletDisconnect(object sender, EventArgs e) { }
        public void OnDaemonSynced(object sender, EventArgs e) { }
        public void OnWalletSynced(object sender, EventArgs e) { }

        public static void Main(string[] args)
        {
            new Program().Run();
        }

        public async void Run()
        {
            // Create a new session
            _session = new TurtleCoin();
            _session.Daemon.RefreshRate = 5000;
            _session.Wallet.RefreshRate = 5000;

            // Assign daemon event handlers
            _session.Daemon.Log += DaemonLog;
            _session.Daemon.Error += Error;
            _session.Daemon.OnConnect += OnDaemonConnect;
            _session.Daemon.OnSynced += OnDaemonSynced;
            _session.Daemon.OnUpdate += OnDaemonUpdate;
            _session.Daemon.OnDisconnect += OnDaemonDisconnect;

            // Assign wallet event handlers
            _session.Wallet.Log += WalletLog;
            _session.Wallet.Error += Error;
            _session.Wallet.OnConnect += OnWalletConnect;
            _session.Wallet.OnSynced += OnWalletSynced;
            _session.Wallet.OnUpdate += OnWalletUpdate;
            _session.Wallet.OnDisconnect += OnWalletDisconnect;

            // Initialize daemon
            await _session.Daemon.InitializeAsync("c:/turtlecoin/turtlecoind.exe", 11898);

            // Begin daemon update loop
            await _session.Daemon.BeginUpdateAsync();
            
            // Initialize wallet
            await _session.Wallet.CreateOrInitializeAsync(_session.Daemon, "c:/turtlecoin/walletd.exe", "c:/turtlecoin/testwallet.wallet", "12345", 8070);

            // Begin wallet update loop
            await _session.Wallet.BeginUpdateAsync();

            // Await input to exit session
            Console.ReadLine();

            // Clean up session, force an exit
            await _session.Exit(true);

            // Await key press to close
            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }
    }
}
