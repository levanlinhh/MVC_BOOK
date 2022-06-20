using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhuongNamBookStore.Models;

namespace PhuongNamBookStore.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }

        //Tao doi tuong data chua dữ liệu từ model dbBansach đã tạo. 
        dbQLBanSachDataContext data = new dbQLBanSachDataContext();
        //Lay gio hang
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Neu gio hang chua ton tai thi khoi tao listGioHang
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        //Them hang vao gio
        public ActionResult ThemGioHang(int id, string strURL)
        {
            //Lay ra Session gio hang
            List<GioHang> lstGioHang = LayGioHang();
            //Kiem tra sách này tồn tại trong Session["GioHang"] chưa?
            GioHang sanpham = lstGioHang.Find(n => n.iMasach == id);
            if (sanpham == null)
            {
                sanpham = new GioHang(id);
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        //Tinh tong tien
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        //HIen thi giỏ hàng.
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            if (lstGioHang.Count == 0)
                return RedirectToAction("Index", "BookStore");
            else
                return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            return PartialView();
        }
        //Xoa GioHang
        public ActionResult XoaGioHang(int id)
        {
            //Lay gio hang tu Session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiem tra sach da co trong Session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasach == id);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMasach == id);
                return RedirectToAction("GioHang");

            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            return RedirectToAction("GioHang");
        }
        //Xóa tất cả giỏ hàng
        public ActionResult XoatatcaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "BookStore");
        }
        //Cap nhat Giỏ hàng
        public ActionResult CapnhatGioHang(int id, FormCollection f)
        {

            //Lay gio hang tu Session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiem tra sach da co trong Session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasach == id);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang", "GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
                return RedirectToAction("Dangnhap", "Nguoidung");
            if (Session["GioHang"] == null)
                return RedirectToAction("Index", "BookStore");


            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGioHang);
        }
        [HttpPost]
        //Xay dung chuc nang Dathang
        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<GioHang> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang            
            foreach (var item in gh)
            {
                CHITIETDONHANG ctdh = new CHITIETDONHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSach = item.iMasach;
                ctdh.SoLuong = item.iSoluong;
                ctdh.DonGia = (decimal)item.dDongia;
                data.CHITIETDONHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("Xacnhandonhang", "GioHang");
        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}