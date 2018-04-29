# TurtleCoin.Net API

An asynchronous wrapper API for loading and accessing the TurtleCoin daemon and wallet applications. Built to ease the creation of wallets and applications that want to pull data from the network.

# Using the API

To begin using the API, you must first include the Newstonsoft Json library in your project. You can find this on Nuget.

Your next step is to create a session

```C#
TurtleCoin _session = new TurtleCoin();
```

From here you can create a daemon connection

```C#
// Create a new session
_session = new TurtleCoin();

// Set how often the network information should be updated (in milliseconds)
_session.Daemon.RefreshRate = 5000;

// Assign daemon event handlers
_session.Daemon.Log += DaemonLog;
_session.Daemon.Error += Error;
_session.Daemon.OnConnect += OnDaemonConnect;
_session.Daemon.OnReady += OnDaemonReady;
_session.Daemon.OnUpdate += OnDaemonUpdate;
_session.Daemon.OnDisconnect += OnDaemonDisconnect;

// Initialize daemon, port defaults to 11898 if not defined
await _session.Daemon.Initialize("Local path, host address, or IP", Port);

// Begin daemon update loop
await _session.Daemon.BeginUpdateAsync();
```

After a connection is established and the update loop has begun, you can do your processing and requests

```C#
// Triggers when daemon information is updated
// Daemon update event
public void OnDaemonUpdate(object sender, EventArgs e)
{
    // Daemon must read as ready to send requests
    if (!(sender as Daemon).Ready)
        Console.WriteLine("Daemon:\tSyncing - {0} / {1}",
            (sender as Daemon).Height, (sender as Daemon).NetworkHeight);
}
```

Opening a wallet is similar, but requires an existing daemon connection

```C#
// Create a new session
_session = new TurtleCoin();

// Set refresh rates
_session.Daemon.RefreshRate = 5000;
_session.Wallet.RefreshRate = 5000;

// Assign daemon event handlers
_session.Daemon.Log += DaemonLog;
_session.Daemon.Error += Error;
_session.Daemon.OnConnect += OnDaemonConnect;
_session.Daemon.OnReady += OnDaemonReady;
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
await _session.Wallet.InitializeAsync(_session.Daemon, "c:/turtlecoin/walletd.exe", 
    "c:/turtlecoin/testwallet.wallet", "12345", 11911);

// Begin wallet update loop
await _session.Wallet.BeginUpdateAsync();
```

```C#
// Wallet update event
public void OnWalletUpdate(object sender, EventArgs e)
{
    if (!(sender as Wallet).Synced)
        Console.WriteLine("Wallet:\tSyncing - {0} / {1}",
            (sender as Wallet).BlockCount, (sender as Wallet).KnownBlockCount);
    else
        Console.WriteLine("Available Balance: {0}, Locked Amount: {1}",
            (sender as Wallet).AvailableBalance, (sender as Wallet).LockedAmount);
}
```

Sending requests is also easy as pie

```C#
if (_session.Wallet.Synced)
{
    await _session.Wallet.SendRequestAsync(RequestMethod.GET_BALANCE, new JObject {
            ["address"] = "TurtleCoin address"
        }, out JObject Result);
    
    Console.WriteLine("Available Balance: {0}, Locked Amount: {1}",
        Result["availableBalance"], Result["lockedAmount"]);
}
```
