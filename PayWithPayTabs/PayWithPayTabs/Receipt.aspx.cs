﻿using System;
using System.Drawing;
using System.Linq;
using System.Web.UI;

namespace TestPTWebService
{
    public partial class Receipt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Helper.PayTabsSession.EmailAddress == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    if (Helper.PayTabsSession.InvalidSecretKey)
                    {
                        lblErrorMessage.Text = "Invalid Secret Key";
                        return;
                    }
                }
                if (Page.Request.QueryString.Count > 0)
                {
                    GetReceipt(Page.Request.QueryString.ToString());
                }
                else
                {
                    GetLastPaymentDetails();
                }
            }
        }

        protected void GetLastPaymentDetails()
        {
            var objUtility = new Utility();
            Models.VerifyPaymentRequest ObjVerifyPaymentRequest = new Models.VerifyPaymentRequest();

            //Set Payment Active URL to session
            //var activeClient = (Models.Settings)Session["ActiveClient"];
            if (Helper.PayTabsSession.EmailAddress != null)
            {
                ObjVerifyPaymentRequest.MerchantEmail = Helper.PayTabsSession.EmailAddress;
                ObjVerifyPaymentRequest.MerchantPassword = Helper.PayTabsSession.Password;
                ObjVerifyPaymentRequest.ReferenceNumber = Helper.PayTabsSession.LastPaymentReferenceNumber;
                ObjVerifyPaymentRequest.SecretKey = Helper.PayTabsSession.SecretKey;
                Models.VerifyPaymentResponse ObjResponse = objUtility.VerifyPayment(ObjVerifyPaymentRequest);

                string redirectURL = "Receipt.aspx?PageID=" + ObjResponse.pt_invoice_id;
                ClientScript.RegisterStartupScript(GetType(), "Load", "<script type='text/javascript'>window.parent.location.href = '" + redirectURL + "'; </script>");
            }
        }

        protected void GetReceipt(string ss)
        {
            var objUtility = new Utility();
            Models.VerifyPaymentRequest ObjVerifyPaymentRequest = new Models.VerifyPaymentRequest();

            //Set Payment Active URL to session
            //var activeClient = (Models.Settings)Session["ActiveClient"];
            if (Helper.PayTabsSession.EmailAddress != null)
            {
                ObjVerifyPaymentRequest.MerchantEmail = Helper.PayTabsSession.EmailAddress;
                ObjVerifyPaymentRequest.MerchantPassword = Helper.PayTabsSession.Password;
                ObjVerifyPaymentRequest.ReferenceNumber = Helper.PayTabsSession.LastPaymentReferenceNumber;
                ObjVerifyPaymentRequest.SecretKey = Helper.PayTabsSession.SecretKey;

                Helper.PayTabsSession.LastTransactionJson = Helper.PayTabsSession.LastPaymentReferenceNumber;
                //Log to File
                Logger.Info("VerifyPayment - Start", "GetReceipt", ObjVerifyPaymentRequest);
   
                Models.VerifyPaymentResponse ObjResponse = objUtility.VerifyPayment(ObjVerifyPaymentRequest);

                if (Helper.PayTabsSession.PageRequestList != null)
                {
                    var lastPayment = new Models.VerifyPaymentResponse();
                    //var tt = JsonConvert.DeserializeObject<Models.VerifyPaymentResponse>(Helper.PayTabsSession.LastTransactionJson);

                    var sessionLastPayment = Helper.PayTabsSession.PageRequestList.FirstOrDefault(a => a.PaymentReference == Helper.PayTabsSession.LastPaymentReferenceNumber);
                    lastPayment.transaction_id = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "transaction_id");
                    lastPayment.shipping_address = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "shipping_address");
                    lastPayment.shipping_city = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "shipping_city");
                    lastPayment.shipping_state = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "shipping_state");
                    lastPayment.shipping_country = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "shipping_country");
                    lastPayment.shipping_postalcode = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "shipping_postalcode");
                    lastPayment.invoice_id = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "invoice_id");
                    lastPayment.currency = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "currency");
                    lastPayment.customer_name = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "customer_name");
                    lastPayment.amount = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "amount");
                    lastPayment.phone_num = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "phone_num");
                    lastPayment.email = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "email");
                    lastPayment.response_code = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "response_code");
                    lastPayment.detail = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "detail");
                    lastPayment.reference_id = objUtility.ReturnQueryParameterValue(Helper.PayTabsSession.LastTransactionJson, "reference_id");

                    if (lastPayment.customer_name != null)
                    {
                        lblName.Text = sessionLastPayment.CcFirstNname + ' ' + sessionLastPayment.CcLastName;
                        lblProduct.Text = sessionLastPayment.ProductsPerTitle;
                        lblCost.Text = sessionLastPayment.UnitPrice;
                        lblQuantity.Text = sessionLastPayment.Quantity.ToString();

                        lblTotalAmount.Text = sessionLastPayment.Currency + " " + sessionLastPayment.Amount;
                        lblAmount.Text = sessionLastPayment.Currency + " " + sessionLastPayment.Amount;
                        lblAmountDue.Text = sessionLastPayment.Currency + " " + sessionLastPayment.Amount;
                        lblInvoiceNo.Text = lastPayment.invoice_id;
                        
                        lblPhone.Text = sessionLastPayment.Phonenumber;
                        lblTransactionDate.Text = DateTime.Now.Date.ToString();
                        
                        lblShippingAddress.Text = sessionLastPayment.AddressShipping + " " + sessionLastPayment.CityShipping + " " + sessionLastPayment.StateShipping + " " + sessionLastPayment.CountryShipping;
                        //lblShippingAddress.Text = lastPayment.AddressShipping + " " + lastPayment.CityShipping + " " + lastPayment.StateShipping + " " + lastPayment.CountryShipping;
                        lblMessage.Text = ObjResponse.detail + " " + ObjResponse.result;
                        lblEmail.Text = sessionLastPayment.Email;
                        imgMessage.Src = "Content/Images/sucesspayment.png";
                        // lblMessage.Text = "Payment successfully done.";
                    }

                    if (ObjResponse.response_code != "100")
                    {
                        lblErrorMessage.ForeColor = Color.DarkRed;
                        lblMessage.Text = ObjResponse.result + " " + ObjResponse.detail;
                        imgMessage.Src = "Content/Images/error.png";
                    }
                }

                //Log to File
                Logger.Info("VerifyPayment - End", "GetReceipt", ObjResponse);
   
            }
        }
    }
}