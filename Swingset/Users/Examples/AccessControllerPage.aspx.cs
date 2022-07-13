﻿using System;
using System.Web.Security;

namespace Owasp.Esapi.Swingset.Users.Examples
{
	public partial class AccessControllerPage : SwingsetPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (lbActions.Items.Count == 0)
			{
				lbActions.Items.Add("Create");
				lbActions.Items.Add("Read");
				lbActions.Items.Add("Update");
				lbActions.Items.Add("Delete");

				lbResources.Items.Add("Report");
				lbResources.Items.Add("Account");
				lbResources.Items.Add("Profile");
			}
		}

		protected void btnAdd_Click(object sender, EventArgs e)
		{
			var action = lbActions.SelectedValue;
			var resource = lbResources.SelectedValue;
			if (action != null && resource != null)
			{
				lbPermissions.Items.Add(string.Format("{0},{1},{2}", Membership.GetUser().UserName, action, resource));
				Esapi.AccessController.AddRule(Membership.GetUser().UserName, action, resource);
			}
		}

		protected void btnCheck_Click(object sender, EventArgs e)
		{
			var resource = lbResources.SelectedValue;
			var action = lbActions.SelectedValue;
			if (resource != null && action != null)
			{
				if (Esapi.AccessController.IsAuthorized(action, resource))
					lblResult.Text = string.Format("{0} is allowed to {1} {2}", Membership.GetUser().UserName, action,
						resource);
				else
					lblResult.Text = string.Format("{0} is not allowed to {1} {2}", Membership.GetUser().UserName,
						action, resource);
			}
		}

		protected void btnRemove_Click(object sender, EventArgs e)
		{
			if (lbPermissions.SelectedIndex > -1)
			{
				var permission = lbPermissions.SelectedValue.Split(',');
				Esapi.AccessController.RemoveRule(Membership.GetUser().UserName, permission[1], permission[2]);
				lbPermissions.Items.Remove(lbPermissions.SelectedItem);
			}
		}
	}
}