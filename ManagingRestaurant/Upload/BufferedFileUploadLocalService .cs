using Google.Protobuf.Reflection;
using ManagingRestaurant.Data;
using Microsoft.AspNetCore.Hosting;
using System.Security.Principal;

namespace ManagingRestaurant.Upload
{
    public class BufferedFileUploadLocalService : IBufferedFileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BufferedFileUploadLocalService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> UploadFile(IFormFile file)
        {
            string path = "";
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadedFiles"));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return file.FileName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }

        public async Task<string> DeleteFile(string url)
        {
            // Lấy đường dẫn tuyệt đối tới file trong thư mục wwwroot/UploadedFiles
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "UploadedFiles", url);

            // Kiểm tra xem file có tồn tại không
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    // Xóa file bất đồng bộ
                    await Task.Run(() => System.IO.File.Delete(filePath));
                    return "success";
                }
                catch (Exception ex)
                {
                    // Trả về thông báo lỗi nếu có
                    return $"failed: {ex.Message}";
                }
            }
            else
            {
                return "failed";
            }
        }

        public async Task<List<string>> UploadManyFiles(IFormFile[] files)
        {
            List<string> uploadedFilePaths = new List<string>();

            if (files == null || files.Length == 0)
            {
                return uploadedFilePaths;
            }

            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "UploadedFiles");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(uploadPath, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Thêm đường dẫn của file vừa upload vào danh sách
                    uploadedFilePaths.Add(Path.Combine("UploadedFiles", file.FileName).Replace("\\", "/"));
                }
            }

            return uploadedFilePaths; // Trả về danh sách các đường dẫn file đã upload
        }

        public async Task<List<string>> DeleteManyFiles(List<string> urls)
        {
            List<string> deletedFiles = new List<string>();

            if (urls == null || urls.Count == 0)
            {
                return deletedFiles;
            }

            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "UploadedFiles");

            foreach (var url in urls)
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, url);

                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        // Xóa file bất đồng bộ
                        await Task.Run(() => System.IO.File.Delete(filePath));
                        deletedFiles.Add(url); // Thêm file đã xóa vào danh sách
                    }
                    catch (Exception ex)
                    {
                        // Bạn có thể xử lý lỗi nếu cần
                        continue;
                    }
                }
            }

            return deletedFiles; // Trả về danh sách các file đã xóa
        }
    }
}
