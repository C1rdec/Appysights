using System;
using System.IO;
using AppDataFileManager;
using Microsoft.Win32;

namespace Appysights.Services
{
    public abstract class AppysightFileBase<TEntity> : AppDataFileBase<TEntity>
        where TEntity : class, new()
    {
        #region Properties

        protected override string FolderName => "Appysights";

        #endregion

        #region Methods

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

        #endregion
    }
}
