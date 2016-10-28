using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Exceptionless;
using Exceptionless.Models;
using Utilities;
using System.IO;
using System.Configuration;
namespace Utilities
{
    public class EmailHelper
    {
        public EmailHelper()
        {

        }
     
        public string BuildEmailMBC(string Body)
        {
            var template = @"
              <style>
			    html, body, div, span, applet, object, iframe, h1, h2, h3, h4, h5, h6, p, blockquote, pre, a, abbr, acronym, address, big, cite, code, del, dfn, em, font, img, ins, kbd, q, s, samp, small, strike, strong, sub, sup, tt, var, dl, dt, dd, ol, ul, li, fieldset, form, label, legend, table, caption, tbody, tfoot, thead, tr, th, td {
			    margin:0; padding:0; border:0; outline:0; font-weight:inherit; font-style:inherit; font-family:inherit; font-size:100%; vertical-align:baseline; }
			    td {vertical-align:top}
			    h1,h2,h3,h4,h5,h6,strong {font-weight:bold;} h1,h2,h3,h4,h5,h6 {margin:.25em 0;}
			    h1 {font-size:120%;} h2 {font-size:110.0%;} h3 {font-size:108%;} 	h4 {font-size:100%;}
			    em{font-style:italic}
			    ul {margin:5px 0px 0px 35px;padding-left:0px;} ol {;margin:5px 0px 0px 35px;padding-left:0px;} li {padding-bottom:5px}
			    sup {vertical-align:top}
			    table {border-collapse:collapse;border-spacing:0;}
			    fieldset,img {border:0;}
			    hr{height:1px;margin-bottom:5px;border-width:0px}
			    /* Font Settings *//* 10px=77% | 11px=85% | 12px=93% | 13px=100% | 14px=108% | 15px=116% | 16px=123.1% | 17px=131% | 18px=138.5% | 19px=146.5% | 20px=153.9% | 21px=161.6% | 22px=167% | 23px=174% | 24px=182% | 25px=189% | 26px=197  */
			    body{font:13px/1.231 arial,helvetica,clean,sans-serif;*font-size:small;*font:x-small;}
			    select,input,button,textarea,button{font:85% arial,helvetica,clean,sans-serif;}
			    table{font-size:inherit;font:100%;}
			    pre,code,kbd,samp,tt{font-family:monospace;*font-size:108%;line-height:100%;}
			    /* Template Settings */
			    a, a:visited, a:hover, a:active {color:#652d8a}
			    .right{text-align:right;}
			    .center{text-align:center;}
		    </style>
			<table border='0' cellspacing='0' cellpadding='0' width='900' align='center' style='max-width: 900px;border:1px solid #333'>
				<tbody>
					<tr>
						<td style='background:#fff;width:10px' width='10'>&nbsp;</td>
						<td style='background:#fff; color:#FFFFFF;padding:10px'><span style='padding:10px;text-align:left;background:#fff; color:#FFFFFF;vertical-align:bottom;font-size:12px;'><a href='http://www.memorybook.com' alt='Mbc Company' title='Lifeline Services'><img src='http://www.memorybook.com/images/logos/mem_jost_logo280x70.jpg' alt='Mbc Company' title='Lifeline Services' style='border:0px' border='0' width='141' height='48'></a></span></td>
						<td style='padding:10px;text-align:right;background:#fff; color:#FFFFFF;vertical-align:bottom;font-size:12px;' width='230'>&nbsp;</td>
						<td style='background:#fff;width:10px;' width='10'>&nbsp;</td>
					</tr>
                    <tr><td colspan='4' style='height:20px;background:#5D7B9D'></td></tr>
					<tr>
						<td style='background:#fff;'>&nbsp;</td>
						<td colspan='2' style='padding: 20px;background-color:#FFFFFF;color:#333333'>
							<div style='font-size:18px; line-height:130%;font:arial,helvetica,clean,sans-serif;'>"
                                + Body
                            + @"</div>
						</td>
						<td style='background:#fff;'>&nbsp;</td>
					</tr>
				</tbody>
			</table>
            ";

            return template;
        }
        public string BuildEmailMeridian(string Body)
        {
            var template = "";
                //@"
   //           <style>
			//    html, body, div, span, applet, object, iframe, h1, h2, h3, h4, h5, h6, p, blockquote, pre, a, abbr, acronym, address, big, cite, code, del, dfn, em, font, img, ins, kbd, q, s, samp, small, strike, strong, sub, sup, tt, var, dl, dt, dd, ol, ul, li, fieldset, form, label, legend, table, caption, tbody, tfoot, thead, tr, th, td {
			//    margin:0; padding:0; border:0; outline:0; font-weight:inherit; font-style:inherit; font-family:inherit; font-size:100%; vertical-align:baseline; }
			//    td {vertical-align:top}
			//    h1,h2,h3,h4,h5,h6,strong {font-weight:bold;} h1,h2,h3,h4,h5,h6 {margin:.25em 0;}
			//    h1 {font-size:120%;} h2 {font-size:110.0%;} h3 {font-size:108%;} 	h4 {font-size:100%;}
			//    em{font-style:italic}
			//    ul {margin:5px 0px 0px 35px;padding-left:0px;} ol {;margin:5px 0px 0px 35px;padding-left:0px;} li {padding-bottom:5px}
			//    sup {vertical-align:top}
			//    table {border-collapse:collapse;border-spacing:0;}
			//    fieldset,img {border:0;}
			//    hr{height:1px;margin-bottom:5px;border-width:0px}
			//    /* Font Settings *//* 10px=77% | 11px=85% | 12px=93% | 13px=100% | 14px=108% | 15px=116% | 16px=123.1% | 17px=131% | 18px=138.5% | 19px=146.5% | 20px=153.9% | 21px=161.6% | 22px=167% | 23px=174% | 24px=182% | 25px=189% | 26px=197  */
			//    body{font:13px/1.231 arial,helvetica,clean,sans-serif;*font-size:small;*font:x-small;}
			//    select,input,button,textarea,button{font:85% arial,helvetica,clean,sans-serif;}
			//    table{font-size:inherit;font:100%;}
			//    pre,code,kbd,samp,tt{font-family:monospace;*font-size:108%;line-height:100%;}
			//    /* Template Settings */
			//    a, a:visited, a:hover, a:active {color:#652d8a}
			//    .right{text-align:right;}
			//    .center{text-align:center;}
		 //   </style>
			//<table border='0' cellspacing='0' cellpadding='0' width='600' align='center' style='max-width: 600px;border:1px solid #333'>
			//	<tbody>
			//		<tr>
			//			<td style='background:#fff;width:10px' width='10'>&nbsp;</td>
			//			<td style='background:#fff; color:#FFFFFF;padding:10px'><span style='padding:10px;text-align:left;background:#fff; color:#FFFFFF;vertical-align:bottom;font-size:12px;'><a href='http://www.memorybook.com' alt='Lifeline Services' title='Lifeline Services'><img src='http://www.memorybook.com/images/logos/mem_jost_logo280x70.jpg' alt='Lifeline Services' title='Lifeline Services' style='border:0px' border='0' width='141' height='48'></a></span></td>
			//			<td style='padding:10px;text-align:right;background:#fff; color:#FFFFFF;vertical-align:bottom;font-size:12px;' width='230'>&nbsp;</td>
			//			<td style='background:#fff;width:10px;' width='10'>&nbsp;</td>
			//		</tr>
   //                 <tr><td colspan='4' style='height:20px;background:#5D7B9D'></td></tr>
			//		<tr>
			//			<td style='background:#fff;'>&nbsp;</td>
			//			<td colspan='2' style='padding: 20px;background-color:#FFFFFF;color:#333333'>
			//				<div style='font-size:18px; line-height:130%;font:arial,helvetica,clean,sans-serif;'>"
   //                             + Body
   //                         + @"</div>
			//			</td>
			//			<td style='background:#fff;'>&nbsp;</td>
			//		</tr>
			//	</tbody>
			//</table>
   //         ";

            return template;
        }
        public void SendEmail(string Subject, string ToAddresses, string CCAddresses,string BccAddress, string Body, Utils.EmailType TypeEmail)

        {
            var brandedHtml = "";
            if (TypeEmail== Utils.EmailType.Mbc)
            {
              brandedHtml = BuildEmailMBC(Body);
            }else if(TypeEmail == Utils.EmailType.Meridian)
            {
                brandedHtml = BuildEmailMeridian(Body);
            }
           
            else
            {
                return;
            }
            var a = ConfigurationManager.AppSettings["smtpuser"].ToString();
            var smtpClient = new SmtpClient();
            //smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpuser"].ToString(), ConfigurationManager.AppSettings["smtppassword"].ToString());
            var mailMessage = new MailMessage
            {
                Subject = Subject,
                Body = brandedHtml,
                IsBodyHtml = true
            };
          
            mailMessage.To.Add(ToAddresses);
            if (!String.IsNullOrEmpty(CCAddresses))
                {
                mailMessage.CC.Add(CCAddresses);
            }
            if (!String.IsNullOrEmpty(BccAddress))
            {
                mailMessage.Bcc.Add(BccAddress);

            }
            try
            {

                 smtpClient.Send(mailMessage);
            }catch(Exception ex)
            {
                //string filePath = @"E:\Logs\Error.txt";

                //using (StreamWriter writer = new StreamWriter(filePath, true))
                //{
                //    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                //       "" + Environment.NewLine  + smtpClient.Host + Environment.NewLine + "Date :" +  DateTime.Now.ToString());
                //    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                //}
                ex.ToExceptionless()
                    .MarkAsCritical()
                    .AddObject(mailMessage)
                     .SetMessage("Error sending receipt email.");
            }
            
        }
    }
}
