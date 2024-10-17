using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using UserRegistration_Project.Models;
using System.Configuration;

namespace UserRegistration_Project.Controllers
{

    public class UserRegCtrlController : Controller
    {


        string connectionString = @"Data Source=RAVI;Initial Catalog=UserRegistrationDB;Integrated Security=True";


        public ActionResult RegisterView()
        {
            ViewBag.States = GetStates();
            ViewBag.Cities = new SelectList(Enumerable.Empty<SelectListItem>(), "CityId", "CityName");
            return View();
        }

        [HttpPost]
        public ActionResult Save(UserRegistrationModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd;
                    if (model.Id > 0) 
                    {
                        cmd = new SqlCommand("UpdateUserRegistration", con);
                    }
                    else
                    {
                        cmd = new SqlCommand("SaveUserRegistration", con);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Mobile", model.Mobile);
                    cmd.Parameters.AddWithValue("@StateId", model.StateId);
                    cmd.Parameters.AddWithValue("@CityId", model.CityId);
                    cmd.Parameters.AddWithValue("@Address", model.Address);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                return Json(new { success = true, message = "User saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        public JsonResult GetCities(int stateId)
        {
            var cities = GetCitiesByState(stateId);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public SelectList GetStates()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT StateId, StateName FROM StateTbl", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                var states = dt.AsEnumerable()
                    .Select(row => new SelectListItem
                    {
                        Value = row["StateId"].ToString(),
                        Text = row["StateName"].ToString()
                    })
                    .ToList();

                return new SelectList(states, "Value", "Text");
            }
        }

        private SelectList GetCitiesByState(int stateId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT CityId, CityName FROM CityTbl WHERE StateId = @StateId", con);
                cmd.Parameters.AddWithValue("@StateId", stateId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                var cities = reader.Cast<IDataRecord>()
                    .Select(r => new SelectListItem
                    {
                        Value = r["CityId"].ToString(),
                        Text = r["CityName"].ToString()
                    })
                    .ToList();

                con.Close();
                return new SelectList(cities, "Value", "Text");
            }
        }

        public JsonResult GetUsers()
        {
            List<UserRegistrationModel> users = new List<UserRegistrationModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, Name, Mobile FROM UserRegistrationTbl", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new UserRegistrationModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Mobile = reader["Mobile"].ToString()
                    });
                }

                con.Close();
            }

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserById(int id)
        {
            UserRegistrationModel user = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, Name, Mobile, StateId, CityId, Address FROM UserRegistrationTbl WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new UserRegistrationModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Mobile = reader["Mobile"].ToString(),
                        StateId = Convert.ToInt32(reader["StateId"]),
                        CityId = Convert.ToInt32(reader["CityId"]),
                        Address = reader["Address"].ToString()
                    };
                }

                con.Close();
            }

            return Json(user, JsonRequestBehavior.AllowGet);
        }

    }
}