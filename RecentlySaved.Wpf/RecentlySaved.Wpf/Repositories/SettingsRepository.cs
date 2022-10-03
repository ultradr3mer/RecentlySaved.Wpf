using Mapster;
using Newtonsoft.Json;
using RecentlySaved.Wpf.Data;
using System.Collections.Generic;
using System.IO;

namespace RecentlySaved.Wpf.Repositories
{
  public class SettingsRepository
  {
    private readonly string fileName = "confing.json";
    private readonly string fileExampleName = "confing.example.json";

    public SettingsRepository()
    {
      this.EnsureExampleConfig();

      if (File.Exists(this.fileName))
      {
        string json = File.ReadAllText(this.fileName);
        SettingsData data = JsonConvert.DeserializeObject<SettingsData>(json);

        data.Adapt(this);
      }
    }

    private void EnsureExampleConfig()
    {
      SettingsData example = new SettingsData()
      {
        PathsToWatch = new List<string> { "%HOMEPATH%\\Downloads", "%HOMEPATH%\\Documents" }
      };

      string exampleJson = JsonConvert.SerializeObject(example);

      if (File.Exists(this.fileExampleName))
      {
        string json = File.ReadAllText(this.fileExampleName);
        if (json == exampleJson)
        {
          return;
        }
        else
        {
          using (var file = File.OpenWrite(fileExampleName))
          {
            using (var writer = new StreamWriter(file))
            {
              writer.WriteAsync(exampleJson);
            }

            return;
          }
        }
      }

      File.WriteAllText(fileExampleName, exampleJson);
    }

    public List<string> PathsToWatch { get; set; } = new List<string>();
  }
}
