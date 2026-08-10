namespace ProForma.Shared.Plugins;

public interface IPluginConfigurationService
{
    public void Load();
    public void Save();
    public IEnumerable<string> GetScanFolders();
    public void AddScanFolder(string newScanFolder);
    public void RemoveScanFolder(string removeScanFolder);
    public IEnumerable<string> GetIndexUrls();
    public void AddIndexUrl(string newIndexUrl);
    public void RemoveIndexUrl(string removeIndexUrl);
    public void Install(PluginConfigurationItem newPlugin);
    public void Deinstall(string id);
    public void Update(PluginConfigurationItem changedPlugin);
    public IEnumerable<PluginConfigurationItem> GetAll();
    public PluginConfigurationItem? GetById(string id);
}