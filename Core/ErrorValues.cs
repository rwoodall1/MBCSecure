using System.Globalization;

namespace Core
{
    public static class ErrorValues
    {
        public static readonly string ContactSupportUserHelp = "An error occurred, please try again later. If the problem persists, contact support";

        public static readonly string UserDoesNotExistInCompanyUserHelp = "This user does not belong to your company.";
        public static readonly string UserUpdateFailedUserMessage = "Failed to edit user.";
        public static readonly string UserUpdateValidationFailedUserMessage = "User update validation failed.";
        public static readonly string UserDeleteFailedUserMessage = "Failed to delete user.";
        public static readonly string UserCreationFailedUserMessage = "Failed to create user.";
        public static readonly string UserCreationValidationFailedUserMessage = "User create validation failed.";
        public static readonly string UserNameAlreadyExistsUserHelp = "A user with this username already exists.";

        public static readonly string UserCrudPermissionsUserHelp = "You do not have a high enough role to perform this action on this user.  If you think this is a mistake, please contact an Administrator";
        public static readonly string CreateUserNotInCompanyUserHelp = "You are attempting to create a user in a different company.";
        public static readonly string UserMustBeAssignedToSalesTeamUserHelp = "This user must be assigned to a Sales Team. Please select a Sales Team and try again.";
        public static readonly string InvalidRoleForSalesTeamAssignmentUserHelp = "A user with this role cannot be assigned to a sales team.";
        public static readonly string CannotAssignRoleToUserUserHelp = "You do not have permission to assign this role to a user.";

        public static readonly string UserDeleteFailedDueToManagerAssignmentUserHelp = "User cannot be deleted because they are assigned as a Manager.";
        public static readonly string AdminsCannotBeDeletedUserHelp = "Unable to delete user because Administrators cannot be deleted.";

        public static readonly string InvalidStateUserHelp = "Please provide a valid 2 letter state code.";

        public static readonly string ValidationFailedUserMessage = "Validation failed for one or more entities.";

        public static readonly string NoValidNladSacNumber = "No valid SAC number exists for that state.";
        public static readonly string NladPhoneNumberGenericError = "A valid phone number could not be generated for this order. Please try again.";

        public static readonly string MissingTpivBypassFields = "Missing required fields for TPIV bypass. Please fill out all sections of TPIV bypass section";

        public static readonly ProcessingError GENERIC_FATAL_BACKEND_ERROR = new ProcessingError(ContactSupportUserHelp, ContactSupportUserHelp, true);

        public static readonly ProcessingError USER_CRUD_PERMISSIONS_ERROR = new ProcessingError("You cannot perform this action on this user.", UserCrudPermissionsUserHelp, false, true);
        public static readonly ProcessingError CREATE_USER_ROLE_ASSIGNMENT_ERROR = new ProcessingError(UserCreationFailedUserMessage, CannotAssignRoleToUserUserHelp, false, true);
        public static readonly ProcessingError CREATE_USER_WITH_EXISTING_USERNAME_ERROR = new ProcessingError(UserCreationFailedUserMessage, UserNameAlreadyExistsUserHelp, false, true);
        public static readonly ProcessingError CREATE_USER_NOT_IN_COMPANY_ERROR = new ProcessingError(UserCreationFailedUserMessage, CreateUserNotInCompanyUserHelp, false, true);
        public static readonly ProcessingError USER_CREATION_FAILED_ERROR = new ProcessingError(UserCreationFailedUserMessage, ContactSupportUserHelp, true);

        public static readonly ProcessingError GET_USER_WITHOUT_ID_ERROR = new ProcessingError("Cannot get user without an Id.", "Please provide a user id and try again", false);
        public static readonly ProcessingError GENERIC_COULD_NOT_FIND_USER_ERROR = new ProcessingError("Failed to find user in company.", "Could not find this user in this company.  If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GENERIC_COULD_NOT_FIND_USERS_ERROR = new ProcessingError("Failed to find any users in company.", "Could not find any users in this company. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GENERIC_COULD_NOT_FIND_MANAGERS_ERROR = new ProcessingError("Failed to find any managers in company.", "Could not find and managers in this company. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError USER_UPDATE_PERMISSIONS_ERROR = new ProcessingError(UserUpdateFailedUserMessage, "You do not have permission to edit this user.  If you think this is a mistake, please contact an Administrator.", false, true);
        public static readonly ProcessingError UPDATE_USER_INVALID_USER_ROLE_ID_PROVIDED_ERROR = new ProcessingError(UserUpdateFailedUserMessage, "Could not update user because the RoleId provided was invalid.", false, true);
        public static readonly ProcessingError GENERIC_UPDATE_USER_FAILED_ERROR = new ProcessingError(UserUpdateFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError COULD_NOT_FIND_USER_TO_UPDATE_ERROR = new ProcessingError(UserUpdateFailedUserMessage, UserDoesNotExistInCompanyUserHelp, false, true);
        public static readonly ProcessingError COULD_NOT_FIND_USER_TO_BEGIN_ORDER_ERROR = new ProcessingError("Order cannot be started.", "Failed to find User to begin order with. If the problem persists, please contact support.", true);
        public static readonly ProcessingError UPDATE_USER_WITH_INVALID_ROW_VERSION_ERROR = new ProcessingError(UserUpdateFailedUserMessage, "This user was edited by someone else while you were editing them.  Please refresh the page and make the desired changes.", false, true);
        public static readonly ProcessingError CANNOT_DELETE_USER_ASSIGNED_AS_MANAGER = new ProcessingError("User cannot be deleted.", UserDeleteFailedDueToManagerAssignmentUserHelp, false);
        public static readonly ProcessingError INVALID_ROLE_ASSIGNMENT_ERROR = new ProcessingError("Error creating/editing user because Role was invalid.", "An error occurred because you attempted to add an invalid role to the user. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GET_LOGGED_IN_USER_INFO_ERROR = new ProcessingError("An error occurred while getting logged in user's info.", "An error occurred while retrieving your info. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_VALIDATE_USER_PERMISSIONS_ERROR = new ProcessingError("An error occurred while validating user permissions.", "An error occurred while trying to ensure you had permission to perform this action. If the problem persists, please contact support.", false);

        public static readonly ProcessingError GENERIC_DELETE_USER_FAILED_ERROR = new ProcessingError(UserDeleteFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError COULD_NOT_FIND_USER_TO_DELETE_ERROR = new ProcessingError(UserDeleteFailedUserMessage, UserDoesNotExistInCompanyUserHelp, false, true);
        public static readonly ProcessingError USER_DELETE_PERMISSIONS_ERROR = new ProcessingError(UserDeleteFailedUserMessage, "You do not have permission to delete this user. If you think this is a mistake, please contact an Administrator", false, true);
        public static readonly ProcessingError USER_ROLE_UPDATE_FAILED_ERROR = new ProcessingError("Failed to update user's role.", ContactSupportUserHelp, true);

       
        public static readonly ProcessingError GENERIC_SEARCH_WITHOUT_CRITERIA_ERROR = new ProcessingError("Failed to search.", "Search could not be completed because no criteria was provided.", false, true);
        public static readonly ProcessingError GENERIC_SEARCH_ERROR = new ProcessingError("An error occurred while completing search.", ContactSupportUserHelp, true);

        public static readonly ProcessingError GENERIC_EXTERNAL_API_RESPONSE_ERROR = new ProcessingError("Failed to determine a valid external API.", "An error occurred while processing you request. If the problem persists, please contact support.", true);
        public static readonly ProcessingError NO_VALID_EXTERNAL_API_FOUND = new ProcessingError("Failed to determine a valid external API", "An error occurred while processing your request. If the problem persists, please contact support.", true);

        public static readonly ProcessingError GENERIC_ADD_TEMP_ORDER_ERROR = new ProcessingError("An error occurred while creating new entry in TempOrders table.", "An error occurred while writing order data to table. If the problem persists, please contact support.", true);

        public static readonly ProcessingError CANNOT_RESET_PASSWORD_FOR_USER_ROLE_ERROR = new ProcessingError("Could not reset password because you don't have the proper permissions.", "Your request could not be processed because you cannot perform this operation on a user with this role.", false);

    }
}
