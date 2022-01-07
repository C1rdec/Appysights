using System;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace Appysights.Services
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

        private string FolderName => "Appysights";

        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        #endregion

        #region Methods

        public void Save() => Save(Entity);

        public void Save(string jsonValue) => Save(Deserialize<TEntity>(jsonValue));

        public void Save(TEntity entity)
        {
            var jsonValue = JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, jsonValue);

            Entity = entity;
        }

        public void Import(Action onSuccess)
        {
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Lurk files (*.lurk)|*.lurk",
            };

            try
            {
                var result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    var text = File.ReadAllText(dialog.FileName);
                    Save(text);
                    onSuccess();
                }
            }
            catch
            {
            }
        }

        protected T Deserialize<T>() => Deserialize<T>(File.ReadAllText(this.FilePath));

        protected T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        #endregion
    }
}
