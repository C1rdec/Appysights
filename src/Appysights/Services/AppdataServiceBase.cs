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
        }

        #endregion

        #region Methods

        public virtual void Initialize()
        {
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

        protected virtual string SubFolderName => string.Empty;

        protected virtual string ImportFileExtension => ".txt";

        protected string FilePath => Path.Combine(FolderPath, FileName);

        protected string FolderPath => Path.Combine(AppDataFolderPath, FolderName, SubFolderName?.Trim() ?? string.Empty);

        private string FolderName => "Appysights";

        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        #endregion

        #region Methods

        public void Save() => Save(Entity);

        public void Save(string jsonValue) => Save(Deserialize<TEntity>(jsonValue));

        public void Save(TEntity entity)
        {
            var name = string.Empty;
            var nameProperty = entity.GetType().GetProperty("Name");
            if (nameProperty != null)
            {
                var oName = nameProperty.GetValue(entity);
                name = oName as string;
            }

            var filePath = string.IsNullOrEmpty(name) ? FilePath: Path.Combine(FolderPath, name);

            HandleSubFolder();
            var jsonValue = JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonValue);

            Entity = entity;
        }

        public void Import(Action onSuccess)
        {
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = $"Files (*{ImportFileExtension})|*{ImportFileExtension}",
            };

            try
            {
                var result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    var text = File.ReadAllText(dialog.FileName);
                    Save(text);
                    onSuccess?.Invoke();
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

        private void HandleSubFolder()
        {
            if (string.IsNullOrEmpty(SubFolderName))
            {
                return;
            }

            var path = Path.Combine(FolderPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        #endregion
    }
}
