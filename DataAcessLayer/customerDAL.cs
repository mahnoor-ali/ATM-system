﻿using Customer_BusinessObj;
using Microsoft.Data.SqlClient;
namespace DataAcessLayer
{
    public class customerDAL
    {
        public bool verifyLogindata(Customer_BO obj)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("u", obj.UserId);
            SqlParameter p2 = new SqlParameter("p", obj.Pin);
            string query = "SELECT * FROM customers WHERE userId=@u AND pinCode =@p ";
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
            return false;
        }

        public void disableUser(string loginId)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("i", loginId);
            string query = "UPDATE customers SET status = 'disabled' WHERE userId=@i";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            comand.ExecuteNonQuery();
            con.Close();
        }
        public Customer_BO checkBalance(Customer_BO obj)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("u", obj.UserId);
            string query = "SELECT balance, accountNo FROM customers WHERE userId=@u";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            SqlDataReader dr = comand.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    obj.balance = Int32.Parse(dr["balance"].ToString());
                    obj.accountNo =  Int32.Parse(dr["accountNo"].ToString());
                }
            }
            con.Close();
            return obj;
        }

        //updates balance in database after withdrawal of money
        public void updateBalance(Customer_BO obj)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            int newBalance = obj.balance - obj.requestedWithdraw;
            SqlParameter p1 = new SqlParameter("b", newBalance);
            SqlParameter p2 = new SqlParameter("u", obj.UserId);
            string query = "UPDATE customers SET balance = @b WHERE userId=@u";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            comand.Parameters.Add(p2);
            comand.ExecuteNonQuery();
            SqlParameter p3 = new SqlParameter("w", obj.requestedWithdraw);
            SqlParameter p4 = new SqlParameter("d", DateTime.Now.Date);
            SqlParameter p5 = new SqlParameter("i", obj.UserId);
            query="INSERT INTO cust_Transactions VALUES(@i, @d ,@w)";
            SqlCommand comand2 = new SqlCommand(query, con);
            comand2.Parameters.Add(p3);
            comand2.Parameters.Add(p4);
            comand2.Parameters.Add(p5);
            comand2.ExecuteNonQuery();
            con.Close();
        }

        //sum all the withdrawals of specified customer on current day
        public Customer_BO withdrawToday(Customer_BO obj)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("u", obj.UserId);
            string query = "SELECT withdrawal FROM cust_Transactions WHERE userId=@u";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            SqlDataReader dr = comand.ExecuteReader();
            int withdrawT = 0;
            while (dr.Read())
            {
                withdrawT+= Int32.Parse(dr["withdrawal"].ToString()); //sum of todays' withdrawals 
            }
            con.Close();
            obj.withdrawalToday=withdrawT;
            return obj;
        }

        //reads the name of account holder to whom sender transfers cash
        public cashReceiver_BO getNameFromDB(cashReceiver_BO receiver)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("n", receiver.accountNo);
            string query = "SELECT name FROM customers WHERE accountNo=@n";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            SqlDataReader dr = comand.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    receiver.name = dr["name"].ToString();
                }
            }
            con.Close();
            return receiver;
        }
        public bool checkExistenceOfAccount(int accountNum)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("n",accountNum);
            string query = "SELECT * FROM customers WHERE accountNo=@n";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            SqlDataReader dr = comand.ExecuteReader();
            if (dr.HasRows)
            {
                con.Close();
                return true;
            }
            return false;
        }

        //transfer cash operation: updates balance of both sender and receiver in database
        public void transferCash(Customer_BO senderObj, cashReceiver_BO receiver)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            // deduct amount from sender's account
            int senderBalnc = senderObj.balance;
            int transaction = senderObj.requestedTransaction;
            string id = senderObj.UserId;
            SqlParameter p1 = new SqlParameter("b", senderBalnc);
            SqlParameter p2 = new SqlParameter("t", transaction);
            SqlParameter p3 = new SqlParameter("i", id);
            string query = "UPDATE customers SET balance=@b - @t WHERE userId=@i";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            comand.Parameters.Add(p2);
            comand.Parameters.Add(p3);
            comand.ExecuteNonQuery();
            // transfer sender's amount to receiver account
            transaction = senderObj.requestedTransaction;
            int accountNum = receiver.accountNo;
            SqlParameter p4 = new SqlParameter("n", accountNum);
            SqlParameter p5 = new SqlParameter("tr", transaction);
            query="UPDATE customers SET balance = balance + @tr  WHERE accountNo = @n";
            SqlCommand comand2 = new SqlCommand(query, con);
            comand2.Parameters.Add(p4);
            comand2.Parameters.Add(p5);
            comand2.ExecuteNonQuery();
            con.Close();
        }

        public void addCashInAccount(Customer_BO b_Obj)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlParameter p1 = new SqlParameter("u", b_Obj.UserId);
            SqlParameter p2 = new SqlParameter("b", b_Obj.balance);
            string query = "UPDATE customers SET balance = @b WHERE userId=@u";
            SqlCommand comand = new SqlCommand(query, con);
            comand.Parameters.Add(p1);
            comand.Parameters.Add(p2);
            comand.ExecuteNonQuery();
            con.Close();
        }

    }
}