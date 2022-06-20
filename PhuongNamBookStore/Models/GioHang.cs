using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhuongNamBookStore.Models
{
    public class GioHang
    {
        //Tao doi tuong data chua dữ liệu từ model dbBansach đã tạo. 
        dbQLBanSachDataContext data = new dbQLBanSachDataContext();
        public int iMasach { set; get; }
        public string sTensach { set; get; }
        public string sAnhbia { set; get; }
        public Double dDongia { set; get; }
        public int iSoluong { set; get; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }

        }
        //Khoi tao gio hàng theo Masach duoc truyen vao voi Soluong mac dinh la 1
        public GioHang(int Masach)
        {
            iMasach = Masach;
            SACH sach = data.SACHes.Single(n => n.Masach == iMasach);
            sTensach = sach.Tensach;
            sAnhbia = sach.Anhbia;
            dDongia = double.Parse(sach.Giaban.ToString());
            iSoluong = 1;
        }
    }
}