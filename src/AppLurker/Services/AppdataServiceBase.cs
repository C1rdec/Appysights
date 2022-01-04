using System;
using System.IO;
using System.Text.Json;

namespace AppLurker.Services
{
    public abstract class AppdataServiceBase<TEntity>
        where TEntity : class, new()
    {
        #region Constructors

        public AppdataServiceBase()
        {
            Entity = new TEntity();
            if (!Directory.Exists(this.FolderPath))
            {
                Directory.CreateDirectory(this.FolderPath);
            }

            if (!File.Exists(this.FilePath))
            {
                Save();
            }
            else
            {
                try
                {
                    Entity = JsonSerializer.Deserialize<TEntity>(File.ReadAllText(FilePath));
                }
                catch
                {
                    File.Delete(FilePath);
                    Entity = new TEntity();
                    Save();
                }
            }
        }

        #endregion

        #region Properties

        public TEntity Entity;

        protected abstract string FileName { get; }

        protected string FilePath => Path.Combine(FolderPath, FileName);

        protected string FolderPath => Path.Combine(AppDataFolderPath, FolderName);

        private string FolderName => "AppLurker";

        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        #endregion

        #region Methods

        public void Save()
        {
            var jsonValue = JsonSerializer.Serialize(Entity, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, jsonValue);
        }

        protected T Deserialize<T>()
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(this.FilePath));
        }

        #endregion
    }
}
