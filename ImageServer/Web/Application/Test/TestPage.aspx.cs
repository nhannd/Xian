using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Admin;
using ClearCanvas.ImageServer.Common.Services.Login;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Test
{
    public partial class TestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CustomIdentity id = SessionManager.Current.User.Identity as CustomIdentity;
            Message.Text = "Welcome <B>"+id.DisplayName + "</B>. Your session will expire on " +
                           SessionManager.Current.Credentials.SessionToken.ExpiryTime;

            DataBind();
            
        }

        public override void DataBind()
        {
            Platform.GetService<IAuthorityAdminServices>(
                delegate(IAuthorityAdminServices service)
                {
                    IList<AuthorityGroupSummary> list =  service.ListAllAuthorityGroups();
                    IList<ListItem> items= CollectionUtils.Map<AuthorityGroupSummary, ListItem>(
                        list,
                        delegate(AuthorityGroupSummary summary)
                            {
                                return new ListItem(summary.Name, summary.AuthorityGroupRef.Serialize());
                            }
                        );
                    NewUserGroupListBox.Items.AddRange(CollectionUtils.ToArray(items));
                });

            base.DataBind();
        }

        protected void Logout(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void ChangePasswordClicked(object sender, EventArgs e)
        {
            LoginCredentials credential = SessionManager.Current.Credentials;
            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                    {
                        try
                        {
                            service.ChangePassword(credential.UserName, OldPassword.Text, NewPassword.Text);
                            ChangePasswordMessage.Text = "Password changed";

                        }
                        catch (Exception ex)
                        {
                            ChangePasswordMessage.Text = ex.Message;
                        }
                    });
            
            
        }

        protected void DeleteUserClicked(object sender, EventArgs e)
        {
            Platform.GetService<IAdminServices>(
                delegate(IAdminServices service)
                {
                    try
                    {
                        service.DeleteUser(DeleteUserName.Text);
                        DeleteUserMessage.Text = "User deleted";

                    }
                    catch (Exception ex)
                    {
                        DeleteUserMessage.Text = ex.Message;
                    }
                });
        }

        protected void ResetPasswordClicked(object sender, EventArgs e)
        {
            Platform.GetService<IAdminServices>(
                delegate(IAdminServices service)
                {
                    try
                    {
                        service.ResetPassword(ResetPasswordUserName.Text);
                        ResetPasswordMessage.Text = "Password reset";

                    }
                    catch (Exception ex)
                    {
                        ResetPasswordMessage.Text = ex.Message;
                    }
                });
        }

        protected void NewUserButtonClicked(object sender, EventArgs e)
        {
            Platform.GetService<IAdminServices>(
                delegate(IAdminServices service)
                {
                    try
                    {
                        UserDetail detail = new UserDetail();
                        detail.UserName = NewUserLoginId.Text;
                        detail.DisplayName = NewUserName.Text;
                        detail.Enabled = true;
                        detail.CreationTime = Platform.Time;
                        detail.ResetPassword = true;

                        foreach (ListItem item in NewUserGroupListBox.Items)
                        {
                            if (item.Selected)
                            {
                                detail.AuthorityGroups.Add(new AuthorityGroupSummary(
                                    new EntityRef(item.Value), item.Text));
                            }
                        } 
                        service.AddUser(detail);

                        NewUserMessage.Text = "User is created";
                    }
                    catch (Exception ex)
                    {
                        NewUserMessage.Text = ex.Message;
                    }
                });
        }

        protected void UpdateUserButtonClicked(object sender, EventArgs e)
        {
            Platform.GetService<IAdminServices>(
                delegate(IAdminServices service)
                {
                    try
                    {
                        UserDetail detail = new UserDetail();
                        detail.UserName = NewUserLoginId.Text;
                        detail.DisplayName = NewUserName.Text;
                        detail.Enabled = true;
                        foreach (ListItem item in NewUserGroupListBox.Items)
                        {
                            if (item.Selected)
                            {
                                detail.AuthorityGroups.Add(new AuthorityGroupSummary(
                                    new EntityRef(item.Value), item.Text));
                            }
                        } 
                        
                        service.UpdateUserDetail(detail);
                        NewUserMessage.Text = "User is updated";
                    }
                    catch (Exception ex)
                    {
                        NewUserMessage.Text = ex.Message;
                    }
                });
        }
    }
}
