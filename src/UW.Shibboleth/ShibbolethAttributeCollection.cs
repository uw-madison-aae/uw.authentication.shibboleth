using System.Collections.Generic;

namespace UW.Shibboleth
{
    /// <summary>
    /// Collection that stores Shibboleth user attributes Shibboleth Attribute Id
    /// </summary>
    public class ShibbolethAttributeCollection : HashSet<string>, IShibbolethAttributeCollection
    {
        /// <summary>
        /// Default collection of UW-Madison Shibboleth attributes
        /// </summary>
        /// <remarks>Last updated 11/23/2022</remarks>
        public static ShibbolethAttributeCollection DefaultUWAttributes = new ShibbolethAttributeCollection([
            "mail",
            "mailAlternateAddress",
            "displayName",
            "cn",
            "sn",
            "l",
            "st",
            "street",
            "o",
            "ou",
            "title",
            "postalAddress",
            "postalCode",
            "physicalDeliveryOfficeName",
            "telephoneNumber",
            "givenName",
            "initials",
            "wiscEduPVI",
            "wiscEduUDDS",
            "wiscEduPhotoID",
            "wiscEduAllEmails",
            "wiscEduPrivacyFlag",
            "wiscEduStudentID",
            "wiscEduPredictedPhotoID",
            "wiscEduHRPersonID",
            "wiscEduISISEmplID",
            "wiscEduAdvisorFlag",
            "wiscEduISISNonStudentEmplid",
            "wiscEduISISStudentEmplID",
            "wiscEduISISAdvisorEmplid",
            "wiscEduWirelessAccess",
            "wiscEduISISInstructorEmplid",
            "wiscEduPortalCSAStudent",
            "wiscEduHRSEmplID",
            "wiscEduManifestSubjectID",
            "wiscEduLibraryPatronID",
            "wiscEduMSOLAddresses",
            "wiscEduPreferredName",
            "wiscEduMSOLPrimaryAddress",
            "wiscEduWiscardAccountNumber",
            "wiscEduSORFirstName",
            "wiscEduSORLastName",
            "eduWisconsinSPVI",
            "eduWisconsinPrincipalName",
            "eduWisconsinHRPersonID",
            "eduWisconsinTelephoneNumberExtension",
            "eduWisconsinLibraryPatronID",
            "eduWisconsinHRSEmplID",
            "eduWisconsinOIMRoles",
            "eduWisconsinLibraryILLiadID",
            "eduWisconsinHRSRoles",
            "eduWisconsinETFMemberID",
            "eppn",
            "affiliation",
            "entitlement",
            "assurance",
            "isMemberOf",
            "eduPersonOrcid"]);

    }

    
}
