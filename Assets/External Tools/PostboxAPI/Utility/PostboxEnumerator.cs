using System;

namespace PostboxAPI
{
    /// <summary>
    /// Enumerator to set communicationtype of the API
    /// </summary>
    public enum PostboxCommunicationType
    {
        xml,
        json // TODO
    }

    /// <summary>
    /// PackageStatus Enumerator. Can be used mulible via bit-operator &
    /// </summary>
    [Flags]
    public enum PostboxDataPackageStatus
    {
        open = 1,
        received = 2,
        processing = 4,
        done = 8,
        error = 16
    }

    /// <summary>
    /// Enumerator, containing the possible Calls
    /// </summary>
    public enum PostboxCallName
    {
        NotFound,
        GetServerTime,
        SendDataPackageToDevice,
        UpdateDataPackageStatus,
        GetDataPackages,
        CheckDeviceID,
        RegisterDevice,
        AddDeviceToFriendlist,
        RemoveDeviceFromFriendlist,
        GetFriendlist,
        GetDataPackageStatus,
        CheckAppID,
        RegisterPlayer,
        PingDevice,
        PingResponse,
        PingStatus
    }

    /// <summary>
    /// CallStatus used by PostboxResponse
    /// </summary>
    public enum PostboxCallStatus
    {
        Undefined,
        Success,
        Fail,
        Error
    }

    /// <summary>
    /// PingStatus for PostboxPingResponse
    /// </summary>
    public enum PostboxPingStatus
    {
        server,
        device
    }

    /// <summary>
    /// This enumerator represents all error codes of the api.
    /// Here a short summery of the current used error codes:
    /// (Please check out http://pba.stephan-schueritz.de/tools/ on the bottom of the script reference for detailed information of the errors)
    /// error_code | modules
    /// - no_data = every call
    /// - parsing_error = every call
    /// - no_callname = every call
    /// - login_invalid = RegisterLicence
    /// - callname_invalid = every call
    /// - api_version_invalid = every call
    /// - internal_error = every call
    /// - no_appid = every call
    /// - appid_invald = every call
    /// - appid_inactiv = every call
    /// - no_deviceid = PingResponse
    /// - deviceid_invaid = GetDataPackages, GetDataPackageStatus,PingDevice, PingResponse, SendDataPackageToDevice
    /// - no_licence = CheckLicenceKey, RegisterLicence
    /// - licence_invalid = CheckLicenceKey, RegisterLicence
    /// - licence_already_taken = CheckLicenceKey, RegisterLicence
    /// - no_sever_connection = Plugin error
    /// - systemtime_error = GetServerTime
    /// - no_requested_appid = CheckAppID, SendDataPackageToDevice
    /// - requested_appid_invalid = CheckAppID, SendDataPackageToDevice, UpdateDataPackageStatus
    /// - requested_appid_inactiv = CheckAppID
    /// - no_requested_deviceid = CheckDeviceID
    /// - requested_deviceid_invalid = CheckDeviceID, SendDataPackageToDevice, UpdateDataPackageStatus
    /// - requested_deviceid_inactiv = CheckDeviceID
    /// - no_params = RegisterDevice, RegisterPlayer, SendDataPackageToDevice, UpdateDataPackageStatus
    /// - error_register_device = RegisterDevice
    /// - params_register_device_invalid = RegisterDevice
    /// - error_register_user = RegisterPlayer
    /// - params_register_user_invalid = RegisterPlayer
    /// - error_register_licence = RegisterLicence
    /// - no_receiver_deviceid = PingDevice,PingStatus
    /// - receiver_deviceid_invalid = PingDevice,PingStatus
    /// - receiver_deviceid_inactiv = PingDevice,PingStatus
    /// - ping_invalid = PingStatus
    /// - no_connection_between_apps = SendDataPackageToDevice
    /// - sender_and_receiver_identic = SendDataPackageToDevice
    /// - transactionid_invalid = GetDataPackageStatus, UpdateDataPackageStatus
    /// - no_transactionid = GetDataPackageStatus
    /// - no_status = UpdateDataPackageStatus
    /// </summary>
    public enum PostboxAPIErrorCode
    {
        no_data = 1,    // Die Anfrage enthält keine Daten
        parsing_error = 2,  // Parsefehler
        no_callname = 3,	// Es wurde kein CallName übergeben
        no_username = 4,	// UserName fehlt
        no_password = 5,	// Passwort fehlt
        login_invalid = 6,	// Anmeldedaten sind ungültig
        callname_invalid = 7,	// CallName wird nicht unterstützt
        api_version_invalid = 8,	// Mitgelieferte API-Versionsnummer nicht korrekt
        internal_error = 9,	// Interner Fehler
        no_appid = 10,	// AppID fehlt
        appid_invald = 11,	// AppID wird nicht unterstützt
        appid_inactiv = 12,	// AppID ist nicht aktiv
        no_deviceid = 13,	// DeviceID fehlt
        deviceid_invaid = 14,	// DeviceID wird nicht unterstützt
        deviceid_inactiv = 15,	// DeviceID ist nicht aktiv
        no_licence = 16,	// Lizenzschlüssel fehlt
        licence_invalid = 17,	// Lizenzschlüssel wird nicht unterstützt
        licence_already_taken = 18,	// Lizenzschlüssel ist bereits vergeben
        no_sever_connection = 50, // Keine Verbindung zum Server möglich.

        systemtime_error = 101, // Systemzeit nicht verfügbar

        no_requested_appid = 1100, // Angefragte AppID fehlt
        requested_appid_invalid = 1101, // Angefragte AppID wird nicht unterstützt
        requested_appid_inactiv = 1102, // Angefragte AppID ist nicht aktiv

        no_requested_deviceid = 1200, // Angefragte DeviceID fehlt
        requested_deviceid_invalid = 1201, // Angefragte DeviceID wird nicht unterstützt
        requested_deviceid_inactiv = 1202, //Angefragte DeviceID ist nicht aktiv

        no_params = 1301, // Keine Parameter
        error_register_device = 1302, // Fehler beim registrieren des Gerätes
        params_register_device_invalid = 1303, // Fehlende Parameter für Geräte-Registrierung

        error_register_user = 1402, // Fehler beim registrieren des Benutzers
        params_register_user_invalid = 1403, // Fehlende Parameter für Benutzer-Registrierung

        error_register_licence = 1501, // Fehler beim registrieren der Lizenz

        no_receiver_deviceid = 1700, // Angefragte Empfänger DeviceID fehlt
        receiver_deviceid_invalid = 1702, // Angefragte Empfänger DeviceID wird nicht unterstützt
        receiver_deviceid_inactiv = 1703, // Angefragte Empfänger DeviceID ist nicht aktiv

        ping_invalid = 1803, // Angefragter Ping ist nicht gefunden

        no_connection_between_devices = 2005, // Es besteht keine Verbindug zwischen den DeviceIDs.
        no_connection_between_apps = 2006, // Kommunikation zwischen den Apps nicht zulässig.

        sender_and_receiver_identic = 2010,  // Sender und Empfänger dürfen nicht identisch sein.

        transactionid_invalid = 2201, // TransactionsID ist ungültig.
        no_transactionid = 2202, // TransactionsID fehlt

        no_status = 2306, // Es wurde kein gültiger Status übergeben.
    }
}