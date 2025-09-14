namespace Softhand.Infrastructure.Services.Abstract;

public interface ISoftApp
{
    static Endpoint Endpoint { get; set; } = new();
    static SoftCall CurrentCall { get; set; } = null!;
    static SoftAccount Account { get; set; }
    static SoftConfig CurrentConfig { get; set; }
    static CallInfo LastCallInfo { get; set; }
    static SoftLogWriter LogWriter { get; set; } = new();
    EpConfig EpConfig { get; set; }
    TransportConfig SipTpConfig { get; set; }
    static AccountCallConfig AccountCallConfig { get; set; }
    static AccountMediaConfig AccountMediaConfig { get; set; }
    static AccountPresConfig AccountPresConfig { get; set; }
    static AccountRegConfig AccountRegConfig { get; set; }
    static AccountSipConfig AccountSipConfig { get; set; }
    static AccountVideoConfig AccountVideoConfig { get; set; }
    static CodecOpusConfig CodecOpusConfig { get; set; }
    static MediaConfig MediaConfig { get; set; }
    static CodecLyraConfig CodecLyraConfig { get; set; }

    void Init(ISoftMonitor monitor, string app_path);
    void Deinit();
    void LoadConfig(string filename);
    void SaveConfig(string filename);
    void BuildAccountConfigs();
}
