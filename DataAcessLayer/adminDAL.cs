﻿using System;
using Microsoft.Data.SqlClient;
using Customer_BusinessObj;

namespace DataAcessLayer
{
    public class adminDAL
    {
        public bool verifyLogindata(admin_BO obj)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("u", obj.UserId);
            SqlParameter p2 = new SqlParameter("p", obj.Pin);
            string query = "SELECT * FROM admins WHERE userId=@u AND pinCode =@p ";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            comand.Parameters.Add(p2);
            SqlDataReader dr = comand.ExecuteReader();
            if (dr.HasRows)
            {
                con.Close();
                return true;
            }
            con.Close();
            dr.Close();
            return false;
        }
        public bool checkIsExist(string Uid)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("u", Uid);
            string query2 = "SELECT * FROM customers WHERE userId=@u";
            SqlCommand comand2 = new SqlCommand(query2, con);
            comand2.Parameters.Add(p1);
            SqlDataReader dr = comand2.ExecuteReader();
            if (dr.HasRows)
            {
                return true;
            }
            return false;
        }
        public int CreateNewAccount(Customer_BO customerAccDetails)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //save new customer account details in database
            SqlParameter p1 = new SqlParameter("u", customerAccDetails.UserId);
            SqlParameter p2 = new SqlParameter("p", customerAccDetails.Pin);
            SqlParameter p3 = new SqlParameter("n", customerAccDetails.holderName);
            SqlParameter p4 = new SqlParameter("t", customerAccDetails.accountType);
            SqlParameter p5 = new SqlParameter("b", customerAccDetails.balance);
            SqlParameter p6 = new SqlParameter("s", customerAccDetails.status);
            string query = "INSERT INTO customers (userId, pinCode, name, accountType, balance, status) VALUES (@u, @p, @n, @t, @b, @s)";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            comand.Parameters.Add(p2);
            comand.Parameters.Add(p3);
            comand.Parameters.Add(p4);
            comand.Parameters.Add(p5);
            comand.Parameters.Add(p6);
            comand.ExecuteNonQuery();
            con.Close();
            return getAccountno(customerAccDetails.UserId);
        }

        public int getAccountno(string id)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con2 = new SqlConnection(conString);
            con2.Open();
            SqlParameter p1 = new SqlParameter("i", id);
            string query2 = $"SELECT accountNo FROM customers WHERE userId=@i";
            SqlCommand comand2 = new SqlCommand(query2, con2);
            comand2.Parameters.Add(p1);
            SqlDataReader dr = comand2.ExecuteReader();
            int accountNo = 0;
            string accNo = "0";
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    accountNo = Int32.Parse(dr["accountNo"].ToString());
                }
            }
          
            con2.Close();
            dr.Close();
            return accountNo;
        }

        public string readAccountHolderName(int accountNum)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("a",accountNum);
            string query = "SELECT name FROM customers WHERE accountNo=@a";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            SqlDataReader dr = comand.ExecuteReader();
            string name = "";
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    name = dr["name"].ToString();
                }
            }
            con.Close();
            dr.Close();
            return name;
        }

        public bool checkExistenceOfAccount(int accountNum)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("a", accountNum);
            string query = "SELECT * FROM customers WHERE accountNo=@a";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            SqlDataReader dr = comand.ExecuteReader();
            if (dr.HasRows)
            {
                con.Close();
                return true;
            }
            con.Close();
            dr.Close();
            return false;
        }

        public string getUserid(int accNo)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("a", accNo);
            string query = "SELECT userId FROM customers WHERE accountNo=@a";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            SqlDataReader dr = comand.ExecuteReader();
            string id = "";
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    id = dr["userId"].ToString();
                }
            }
            return id;
        }

        //delete account from database having specified accountNo
        public bool DeleteExistingAccount(int accountNum)
        {
            //if the account of such account no does not exist, it will definitely not deleted
            if (checkExistenceOfAccount(accountNum))
            {
                string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                SqlConnection con = new SqlConnection(conString);
                con.Open();
                string uId = getUserid(accountNum);
                SqlParameter p0 = new SqlParameter("i", uId);
                string query = "DELETE FROM cust_Transactions WHERE userId=@i";
                SqlCommand comand = new SqlCommand(query, con);
                comand.Parameters.Add(p0);
                comand.ExecuteNonQuery();

                SqlParameter p1 = new SqlParameter("a", accountNum);
                query = "DELETE FROM customers WHERE accountNo=@a";
                SqlCommand comand2 = new SqlCommand(query, con);
                comand2.Parameters.Add(p1);
                comand2.ExecuteNonQuery();
                con.Close();
                return true;
            }
            return false;
        }
        
        public List<Customer_BO> searchAccounts(Customer_BO searchCriteria)
        {
            List<Customer_BO> customerList = new List<Customer_BO>();
            bool[] paramsExist = {false, false, false, false, false, false};
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = null;
            SqlParameter p2 = null;
            SqlParameter p3 = null;
            SqlParameter p4 = null;
            SqlParameter p5 = null;
            SqlParameter p6 = null;
            //apply conditions for only those fields that are entered by admin
            string query = "SELECT accountNo, userId, name, accountType,balance,status FROM customers WHERE ";
            if (searchCriteria.accountNo != -1)
            {
                paramsExist[0]=true;
                p1 = new SqlParameter("a", searchCriteria.accountNo);
                query += " accountNo = @a";
            }
            if (searchCriteria.UserId!="")
            {
                paramsExist[1]=true;
                p2= new SqlParameter("i", searchCriteria.UserId);
                if (paramsExist[0]==true)
                    query+=" AND";
                query+=" userId = @i";
            }
            if (searchCriteria.holderName!="")
            {
                paramsExist[2]=true;
                p3= new SqlParameter("n", searchCriteria.holderName);
                if (paramsExist[1]==true)
                    query+=" AND";
                query+=" name= @n";
            }
            if (searchCriteria.accountType!="")
            {
                paramsExist[3]=true;
                p4= new SqlParameter("t", searchCriteria.accountType);
                if (paramsExist[2]==true)
                    query+=" AND";
                query+=" accountType = @t";
            }
            if (searchCriteria.balance!=-1)
            {
                paramsExist[4]=true;
                p5=new SqlParameter("b", searchCriteria.balance);
                if (paramsExist[3]==true)
                    query+=" AND";
                query+=" balance = @b";
            }
            if (searchCriteria.status!="")
            {
                paramsExist[5]=true;
                p6=new SqlParameter("s", searchCriteria.status);
                if (paramsExist[4]==true)
                    query+=" AND";
                query+=" status = @s";
            }
            SqlCommand comand = new SqlCommand(query, con);
            if (paramsExist[0]==true)
                comand.Parameters.Add(p1);
            if (paramsExist[1]==true)
                comand.Parameters.Add(p2);
            if (paramsExist[2]==true)
                comand.Parameters.Add(p3);
            if (paramsExist[3]==true)
                comand.Parameters.Add(p4);
            if (paramsExist[4]==true)
                comand.Parameters.Add(p5);
             if (paramsExist[5]==true)
                comand.Parameters.Add(p6);

            SqlDataReader dr = comand.ExecuteReader();
            while (dr.Read())
            {
                Customer_BO customer = new Customer_BO();
                customer.accountNo = Int32.Parse(dr[0].ToString());
                customer.UserId=  dr[1].ToString();
                customer.holderName= dr[2].ToString();
                customer.accountType=dr[3].ToString();
                customer.balance=  Int32.Parse(dr[4].ToString());
                customer.status=dr[5].ToString();
                customerList.Add(customer);
            }
            con.Close();
            dr.Close();
            return customerList;
        }

        public Customer_BO getCustomerDetails(int accountNo)
        {
            Customer_BO customerOld = new Customer_BO();
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("n", accountNo);
            string query = "SELECT accountNo, accountType, name, balance, status FROM customers WHERE accountNo=@n";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            SqlDataReader dr = comand.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    customerOld.accountNo =Int32.Parse(dr["accountNo"].ToString());
                    customerOld.accountType = dr["accountType"].ToString();
                    customerOld.holderName = dr["name"].ToString();
                    customerOld.balance = Int32.Parse(dr["balance"].ToString());
                    customerOld.status = dr["status"].ToString();
                }
            }
            con.Close();
            dr.Close();
            return customerOld;
        }

        public void updateCustomer(Customer_BO customer)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("i", customer.UserId);
            SqlParameter p2 = new SqlParameter("p", customer.Pin);
            SqlParameter p3 = new SqlParameter("n", customer.holderName);
            SqlParameter p4 = new SqlParameter("s", customer.status);
            SqlParameter p5 = new SqlParameter("a", customer.accountNo);
            string query = "UPDATE customers SET userId=@i, pinCode=@p, name=@n, status=@s WHERE accountNo=@a";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            comand.Parameters.Add(p2);
            comand.Parameters.Add(p3);
            comand.Parameters.Add(p4);
            comand.Parameters.Add(p5);
            comand.ExecuteNonQuery();
            con.Close();
        }

        public List<Customer_BO> viewReports(int min, int max)
        {
            List<Customer_BO> customerList = new List<Customer_BO>();
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("min", min);
            SqlParameter p2 = new SqlParameter("max", max);

            string query = "SELECT accountNo, userId, name, accountType,balance,status FROM customers WHERE balance>=@min AND balance<=@max ";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            comand.Parameters.Add(p2);
            SqlDataReader dr = comand.ExecuteReader();
            while (dr.Read())
            {
                Customer_BO customer = new Customer_BO();
                customer.accountNo = Int32.Parse(dr[0].ToString());
                customer.UserId=  dr[1].ToString();
                customer.holderName= dr[2].ToString();
                customer.accountType=dr[3].ToString();
                customer.balance=  Int32.Parse(dr[4].ToString());
                customer.status=dr[5].ToString();
                customerList.Add(customer);
            }
            con.Close();
            dr.Close();
            return customerList;
        }

    }
}
