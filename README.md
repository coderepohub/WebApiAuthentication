# Application to Validate JWT token in a web api using Azure AD

## An Application to validate the JWT tokens coming in request header and fetch roles from it.

### Steps to Create an Application in Azure AD :

Make sure first to have an appication in Azure Active Directory, if you don't have one please do follow the following approach : 

1. Visit [Azure Portal](https://portal.azure.com/) and open AD, we are going to create an application, create user and roles and assign user and roles to the application

2. Create an application in Azure AD, Go to App Registration option and click on New application registration 
   ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/1.png "App Registration")

3. Now a new blade will open where it will ask to give some name to the application, give any name, in Application Type option make sure you have selected Web app / API and in Sign-on URL give some random dummy url . Make sure application name and url should be unique across Azure ad domain.
     ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/2.png "Create Application")
     
4. Now once the application has been created , Open your application (Azure AD Application) which you have created, you will see an option to edit the manifest , click on this option 
    ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/3.png "Manifest")
    
5. It will open a JSON file where you will see several fields. In this file there will be one JSON property (array data structure)   *appRoles* , add your roles/role properties inside the appRoles array. Below are the sample format for two roles which has been added
  ```
   "appRoles": [
   		 {
     		 "allowedMemberTypes": [
   		     "User"
   		   ],
  		    "displayName": "INVESTORROLE",
  		    "id": "5461dd28-7a19-4d40-a816-1c1199f77c57",
 		     "isEnabled": true,
 		     "description": "Role for the Investor Type User",
      "value": "INVESTORROLE"
    },
    {
 		     "allowedMemberTypes": [
 		       "User"
 		     ],
  		    "displayName": "DIRECTROLE",
  		    "id": "7f24acb0-65dc-4e0c-9861-2feff6f0d1b2",
  		    "isEnabled": true,
  		    "description": "Role for the Direct Type User",
  		    "value": "DIRECTROLE"
  		  }
  		]
  ```
  
6.	Now roles has been added to the application, it’s time to create users and assign these roles to the user and user to application. Go to User option in azure ad and click on *Users* . It will open User option 
   ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/4.png "Users")
   
7.  Now click on *New User* option , it will open a blade to add new user 
    ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/5.png "New User")

8.  Fill all the user details *Name, User Name, Profile, Properties, Groups, Directory Role* . Make sure in the *Directory Role* you must select *Global administrator* Directory role.
    ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/6.png "Assign Role")
    
9.  Once the user will be created it’s time to assign the user to the role and to the application.Go to the Azure Ad main menu and select **Enterprise applications** option 
    ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/7.png "Enterprise Application")

10.	It will show list of applications , click on the application which we created and then click on **Users and groups** 
    ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/8.png "Users and Groups")
    
11.	Click on Add user, it will add that user to the application.
     ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/9.png "Users and Groups")
     
12. Select the User from the *Users* option and select role from *Select Role* option
    ![picture alt](https://github.com/coderepohub/WebApiAuthentication/blob/master/image_content/10.png "Users and Roles")
    
Following all the above steps will successfully configure the application with users and roles. Now whenever we will generate token for this application for the assigned user it will have the role assigned to it.

### Now we can download the code change the settings from web.config file


- NOTE : IT ALSO HAS CODE TO QUERY COSMOS DB.

Please do update the Web.Config file
