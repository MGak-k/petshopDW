using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Helpers
{
    public static class FileHelper
    {
        public static bool ByteArrayToFile(string FilePath, byte[] File)
        {
            bool result = false;
            System.IO.FileStream fileStream = null;

            try
            {
                fileStream = new System.IO.FileStream(FilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                fileStream.Write(File, 0, File.Length);
                fileStream.Close();

                result = true;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

            return result;
        }
    }
}
