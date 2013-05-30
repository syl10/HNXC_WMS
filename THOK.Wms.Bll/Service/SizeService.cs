using System;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using System.Linq;

namespace THOK.Wms.Bll.Service
{
    public class SizeService : ServiceBase<Size>, ISizeService
    {
        [Dependency]
        public ISizeRepository SizeRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows,string SizeName, string SizeNo)
        {
            IQueryable<Size> sizeQuery = SizeRepository.GetQueryable();
            var size = sizeQuery.Where(s => s.SizeName.Contains(SizeName))
                .OrderBy(s => s.ID).AsEnumerable()
                .Select(s => new
                {
                    s.ID,
                    s.SizeName,
                    s.SizeNo,
                    s.Length,
                    s.Width,
                    s.Height
                });
            if(SizeNo != "" && SizeNo!=null)
            {
                int sizeno = -1;
                try { sizeno = Convert.ToInt32(SizeNo); }
                catch { sizeno = -1; }
                finally 
                {
                    size = size.Where(s => s.SizeNo == sizeno)
                        .OrderBy(s => s.ID).AsEnumerable()
                        .Select(s => new
                        {
                            s.ID,
                            s.SizeName,
                            s.SizeNo,
                            s.Length,
                            s.Width,
                            s.Height
                        });
                }
            }
            
            int total = size.Count();
            size = size.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = size.ToArray() };
        }

        public bool Add(Size size, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var si = new Size();
            if (si != null)
            {
                try
                {
                    si.SizeName = size.SizeName;
                    si.SizeNo = size.SizeNo;
                    si.Length = size.Length;
                    si.Width = size.Width;
                    si.Height = size.Height;

                    SizeRepository.Add(si);
                    SizeRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "原因：找不到当前登陆用户！请重新登陆！";
            }
            return result;
        }

        public bool Save(Size size, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var si = SizeRepository.GetQueryable().FirstOrDefault(s => s.ID == size.ID);

            if (si != null)
            {
                try
                {
                    si.SizeName = size.SizeName;
                    si.SizeNo = size.SizeNo;
                    si.Length = size.Length;
                    si.Width = size.Width;
                    si.Height = size.Height;

                    SizeRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "原因：未找到当前需要修改的数据！";
            }
            return result;
        }

        public bool Delete(int sizeId, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var si = SizeRepository.GetQueryable().FirstOrDefault(s => s.ID == sizeId);
            if (si != null)
            {
                try
                {
                    SizeRepository.Delete(si);
                    SizeRepository.SaveChanges();
                    result = true;
                }
                catch (Exception)
                {
                    strResult = "原因：已在使用";
                }
            }
            else
            {
                strResult = "原因：未找到当前需要删除的数据！";
            }
            return result;
        }

        public object GetSize(int page, int rows, string queryString, string value)
        {
            string id = "", sizeName = "";

            if (queryString == "id")
            {
                id = value;
            }
            else
            {
                sizeName = value;
            }
            IQueryable<Size> sizeQuery = SizeRepository.GetQueryable();
            int Id = Convert.ToInt32(id);
            var size = sizeQuery.Where(si => si.ID == Id && si.SizeName.Contains(sizeName))
                .OrderBy(si => si.ID).AsEnumerable().
                Select(si => new
                {
                    si.ID,
                    si.SizeName,
                    si.SizeNo
                });
            int total = size.Count();
            size = size.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = size.ToArray() };
        }

        public System.Data.DataTable GetSize(int page, int rows, string sizeName)
        {
            IQueryable<Size> sizeQuery = SizeRepository.GetQueryable();
            var size = sizeQuery.Where(si => si.SizeName.Contains(sizeName))
                .OrderBy(si => si.ID).AsEnumerable()
                .Select(si => new
                {
                    si.ID,
                    si.SizeName,
                    si.SizeNo,
                    si.Length,
                    si.Width,
                    si.Height
                });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("尺寸ID", typeof(string));
            dt.Columns.Add("尺寸名称", typeof(string));
            dt.Columns.Add("尺寸编号", typeof(string));
            dt.Columns.Add("长度", typeof(string));
            dt.Columns.Add("宽度", typeof(string));
            dt.Columns.Add("高度", typeof(string));
            foreach (var item in size)
            {
                dt.Rows.Add
                    (
                        item.ID,
                        item.SizeName,
                        item.SizeNo,
                        item.Length,
                        item.Width,
                        item.Height
                    );
            }
            return dt;
        }
    }
}
