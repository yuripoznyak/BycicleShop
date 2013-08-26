using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Security;
using BycicleShop.Common.SqlContext.Concrete;
using BycicleShop.Common.SqlContext.Entities;
using WebMatrix.WebData;

namespace Itransition.Providers
{
    public class CustomMembershipProvider : ExtendedMembershipProvider
    {
        #region Properties

        public override string ApplicationName
        {
            get { return GetType().Assembly.GetName().Name; }
            set { ApplicationName = GetType().Assembly.GetName().Name; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 10; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 0; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return String.Empty; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        #endregion Properties

        #region Functions

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        /// <param name="username">The user name for the new user. </param><param name="password">The password for the new user. </param><param name="email">The e-mail address for the new user.</param><param name="passwordQuestion">The password question for the new user. (use "" for this field)</param><param name="passwordAnswer">The password answer for the new user (use "" for this field)</param><param name="isApproved">Whether or not the new user is approved to be validated.</param><param name="providerUserKey">The unique identifier from the membership data source for the user.</param><param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string passwordQuestion, string passwordAnswer, bool isApproved,
                                                  object providerUserKey, out MembershipCreateStatus status)
        {
            if (String.IsNullOrEmpty(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (String.IsNullOrEmpty(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (String.IsNullOrEmpty(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            string hashedPassword = Crypto.HashPassword(password);
            if (hashedPassword.Length > 128)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            using (var context = new UsersDataContext())
            {
                if (context.Entity.Any(usr => usr.UserName == username))
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }
                if (context.Entity.Any(usr => usr.EmailAdress == email))
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
                var user = new User
                {
                    UserName = username,
                    EmailAdress = email,
                    Password = hashedPassword,
                    IsApproved = isApproved,
                    CreateDate = DateTime.UtcNow,
                    LastPasswordChangedDate = DateTime.UtcNow,
                    PasswordFailuresSinceLastSuccess = 0,
                    LastLoginDate = DateTime.UtcNow,
                    LastActivityDate = DateTime.UtcNow,
                    LastLockoutDate = DateTime.UtcNow,
                    IsLockedOut = false,
                    LastPasswordFailureDate = DateTime.UtcNow,
                    ConfirmationToken = "",
                    PasswordVerificationToken = ""
                };
                context.Entity.Add(user);
                context.SaveChanges();
                status = MembershipCreateStatus.Success;
                return new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId, user.EmailAdress, null,
                                          null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value,
                                          user.LastLoginDate.Value, user.LastActivityDate.Value,
                                          user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value);
            }
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        /// <param name="username">The name of the user to validate. </param><param name="password">The password for the specified user. </param>
        public override bool ValidateUser(string username, string password)
        {
            if (String.IsNullOrEmpty(username))
            {
                return false;
            }
            if (String.IsNullOrEmpty(password))
            {
                return false;
            }

            using (var context = new UsersDataContext())
            {
                var user = context.Entity.FirstOrDefault(x => x.UserName == username);
                if (user == null)
                {
                    return false;
                }
                if (!user.IsApproved)
                {
                    return false;
                }
                if (user.IsLockedOut)
                {
                    return false;
                }

                var hashedPassword = user.Password;
                bool verificationSucceeded = (hashedPassword != null &&
                                              Crypto.VerifyHashedPassword(hashedPassword, password));
                if (!verificationSucceeded)
                {
                    int failures = user.PasswordFailuresSinceLastSuccess;
                    if (failures < MaxInvalidPasswordAttempts)
                    {
                        user.PasswordFailuresSinceLastSuccess += 1;
                        user.LastPasswordFailureDate = DateTime.UtcNow;
                    }
                    else if (failures >= MaxInvalidPasswordAttempts)
                    {
                        user.LastPasswordFailureDate = DateTime.UtcNow;
                        user.LastLockoutDate = DateTime.UtcNow;
                        user.IsLockedOut = true;
                    }
                }
                else
                {
                    user.PasswordFailuresSinceLastSuccess = 0;
                    user.LastLoginDate = DateTime.UtcNow;
                    user.LastActivityDate = DateTime.UtcNow;
                }
                context.SaveChanges();
                return verificationSucceeded;
            }
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param><param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey is int)
            {
            }
            else return null;

            using (var context = new UsersDataContext())
            {
                var user = context.Entity.Find(providerUserKey);
                if (user != null)
                {
                    if (userIsOnline)
                    {
                        user.LastActivityDate = DateTime.UtcNow;
                        context.SaveChanges();
                    }
                    return new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId, user.EmailAdress,
                                              null, null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value,
                                              user.LastLoginDate.Value, user.LastActivityDate.Value,
                                              user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="username">The name of the user to get information for. </param><param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user. </param>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (String.IsNullOrEmpty(username))
            {
                return null;
            }
            using (var context = new UsersDataContext())
            {
                var user = context.Entity.FirstOrDefault(x => x.UserName == username);
                if (user != null)
                {
                    if (userIsOnline)
                    {
                        user.LastActivityDate = DateTime.UtcNow;
                        context.SaveChanges();
                    }
                    return new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId, user.EmailAdress,
                                              null, null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value,
                                              user.LastLoginDate.Value, user.LastActivityDate.Value,
                                              user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value);
                }
            }
            return null;
        }

        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        /// <param name="username">The user to update the password for. </param><param name="oldPassword">The current password for the specified user. </param><param name="newPassword">The new password for the specified user. </param>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(username))
            {
                return false;
            }
            if (String.IsNullOrEmpty(oldPassword))
            {
                return false;
            }
            if (String.IsNullOrEmpty(newPassword))
            {
                return false;
            }

            using (var context = new UsersDataContext())
            {
                var user = context.Entity.FirstOrDefault(x => x.UserName == username);
                if (user == null)
                {
                    return false;
                }

                var hashedPassword = user.Password;
                bool verificationSucceeded = (hashedPassword != null &&
                                              Crypto.VerifyHashedPassword(hashedPassword, oldPassword));
                if (verificationSucceeded)
                {
                    user.PasswordFailuresSinceLastSuccess = 0;
                }
                else
                {
                    int failures = user.PasswordFailuresSinceLastSuccess;
                    if (failures < MaxInvalidPasswordAttempts)
                    {
                        user.PasswordFailuresSinceLastSuccess += 1;
                        user.LastPasswordFailureDate = DateTime.UtcNow;
                    }
                    else if (failures >= MaxInvalidPasswordAttempts)
                    {
                        user.LastPasswordFailureDate = DateTime.UtcNow;
                        user.LastLockoutDate = DateTime.UtcNow;
                        user.IsLockedOut = true;
                    }
                    context.SaveChanges();
                    return false;
                }
                var newHashedPassword = Crypto.HashPassword(newPassword);
                if (newHashedPassword.Length > 128)
                {
                    return false;
                }
                user.Password = newHashedPassword;
                user.LastPasswordChangedDate = DateTime.UtcNow;
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        public override bool UnlockUser(string userName)
        {
            using (var context = new UsersDataContext())
            {
                var user = context.Entity.FirstOrDefault(x => x.UserName == userName);
                if (user != null)
                {
                    user.IsLockedOut = false;
                    user.PasswordFailuresSinceLastSuccess = 0;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>
        /// The number of users currently accessing the application.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            var dateActive =
                DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(Convert.ToDouble(Membership.UserIsOnlineTimeWindow)));
            using (var context = new UsersDataContext())
            {
                return context.Entity.Count(x => x.LastActivityDate > dateActive);
            }
        }

        /// <summary>
        /// Removes a user from the membership data source. 
        /// </summary>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        /// <param name="username">The name of the user to delete.</param><param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (String.IsNullOrEmpty(username))
            {
                return false;
            }
            using (var context = new UsersDataContext())
            {
                var user = context.Entity.FirstOrDefault(x => x.UserName == username);
                if (user != null)
                {
                    context.Entity.Remove(user);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        /// <param name="email">The e-mail address to search for. </param>
        public override string GetUserNameByEmail(string email)
        {
            using (var context = new UsersDataContext())
            {
                var user = context.Entity.FirstOrDefault(x => x.EmailAdress == email);
                return user != null ? user.UserName : string.Empty;
            }
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        /// <param name="emailToMatch">The e-mail address to search for.</param><param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();
            using (var context = new UsersDataContext())
            {
                totalRecords = context.Entity.Count(x => x.EmailAdress == emailToMatch);
                var users =
                    context.Entity.Where(x => x.EmailAdress == emailToMatch)
                           .OrderBy(x => x.UserName)
                           .Skip(pageIndex * pageSize)
                           .Take(pageSize);
                foreach (var user in users)
                {
                    membershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId,
                                                           user.EmailAdress, null, null, user.IsApproved,
                                                           user.IsLockedOut, user.CreateDate.Value,
                                                           user.LastLoginDate.Value, user.LastActivityDate.Value,
                                                           user.LastPasswordChangedDate.Value,
                                                           user.LastLockoutDate.Value));
                }
            }
            return membershipUsers;
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        /// <param name="usernameToMatch">The user name to search for.</param><param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();
            using (var context = new UsersDataContext())
            {
                totalRecords = context.Entity.Count(x => x.UserName.Contains(usernameToMatch));
                var users =
                    context.Entity.Where(x => x.UserName == usernameToMatch)
                           .OrderBy(x => x.UserName)
                           .Skip(pageIndex * pageSize)
                           .Take(pageSize);
                foreach (var user in users)
                {
                    membershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId,
                                                           user.EmailAdress, null, null, user.IsApproved,
                                                           user.IsLockedOut, user.CreateDate.Value,
                                                           user.LastLoginDate.Value, user.LastActivityDate.Value,
                                                           user.LastPasswordChangedDate.Value,
                                                           user.LastLockoutDate.Value));
                }
            }
            return membershipUsers;
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();
            using (var context = new UsersDataContext())
            {
                totalRecords = context.Entity.Count();
                var users = context.Entity.OrderBy(x => x.UserName).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (var user in users)
                {
                    membershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId,
                                                           user.EmailAdress, null, null, user.IsApproved,
                                                           user.IsLockedOut, user.CreateDate.Value,
                                                           user.LastLoginDate.Value, user.LastActivityDate.Value,
                                                           user.LastPasswordChangedDate.Value,
                                                           user.LastLockoutDate.Value));
                }
            }
            return membershipUsers;
        }

        #endregion

        #region Not Supported

        //CustomMembershipProvider does not support password retrieval scenarios.
        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CustomMembershipProvider does not support password reset scenarios.
        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CustomMembershipProvider does not support question and answer scenarios.
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CustomMembershipProvider does not support UpdateUser because this method is useless.
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }

        public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation,
                                                    IDictionary<string, object> values)
        {
            throw new NotImplementedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteAccount(string userName)
        {
            throw new NotImplementedException();
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
        {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public override bool IsConfirmed(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreateDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastPasswordFailureDate(string userName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
