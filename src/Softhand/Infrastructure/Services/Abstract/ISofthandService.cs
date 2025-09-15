namespace Softhand.Infrastructure.Services.Abstract;

public interface ISofthandService
{
    Endpoint Endpoint { get; set; } 
    SoftCall CurrentCall { get; set; } 
    SoftAccount Account { get; set; }
    SoftConfig CurrentConfig { get; set; }
    CallInfo LastCallInfo { get; set; }
    SoftLogWriter LogWriter { get; set; } 
    SoftAccountConfigModel SoftAccountConfig { get; set; }
    static ISoftMonitor Monitor { get; set; }
    void Init(ISoftMonitor monitor, string app_path);
    void Deinit();
    void LoadConfig(string filename);
    void SaveConfig(string filename);
    void BuildAccountConfigs();
}
