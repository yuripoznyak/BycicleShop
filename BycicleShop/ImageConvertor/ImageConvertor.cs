using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageConvertor
{
    public class ImageConvertor
    {
        private const string TypeRule = "gifjpgjpeg";
        public string BytecodeToImage(byte[] bytecode, string fileName)
        {
            if (bytecode == null)
            {
                throw new Exception("Bytecode is empty");
            }

            const string path = @"D:\";
                
            using (var stream = new MemoryStream(bytecode))
            {
                var newImage = Image.FromStream(stream);
                newImage.Save(path + fileName);
            }


            return path + fileName;
        }

        public byte[] ImageToBytecode(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new Exception("Incorrect image path");
            }
            
            var fileInfo = new FileInfo(imagePath);
            if (!TypeRule.Contains(fileInfo.Extension))
            {
                throw new Exception("Incorrect image extention");
            }
            
            byte[] bytecode;
            using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read) )
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    bytecode = binaryReader.ReadBytes((int)fileStream.Length);
                }
            }

            if (bytecode.Length <= 0)
            {
                throw new Exception("Image is empty o_O");
            }

            return bytecode;
        }
    }
}
