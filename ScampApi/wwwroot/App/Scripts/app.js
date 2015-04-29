'use strict';
angular.module('scamp', ['ngRoute', 'AdalAngular', 'ui.bootstrap', 'infinite-scroll'])
.config(['$routeProvider', '$httpProvider', 'adalAuthenticationServiceProvider', '$locationProvider', function ($routeProvider, $httpProvider, adalProvider, $locationProvider) {

    $routeProvider.when("/Home", {
        controller: "homeCtrl",
        templateUrl: "/App/Views/Home.html",
    }).when("/dashboard", {
        controller: "dashboardCtrl",
        templateUrl: "/App/Views/Dashboard.html",
        requireADLogin: true
    }).when("/resources", {
        controller: "resourcesCtrl",
        templateUrl: "/App/Views/Resources.html",
        requireADLogin: true
    }).when("/logs", {
        controller: "logsCtrl",
        templateUrl: "/App/Views/Logs.html",
        requireADLogin: true
    }).when("/settings", {
        controller: "settingsCtrl",
        templateUrl: "/App/Views/Settings.html",
        requireADLogin: true
    }).when("/settings/admins", {
        controller: "settingsAdminsCtrl",
        templateUrl: "/App/Views/SettingsAdmins.html",
        requireADLogin: true
    }).when("/settings/subs/edit/", {
        controller: "settingsSubSetupCtrl",
        templateUrl: "/App/Views/SettingsSubSetup.html",
        requireADLogin: true
    }).when("/settings/subs/edit/:subId", {
        controller: "settingsSubSetupCtrl",
        templateUrl: "/App/Views/SettingsSubSetup.html",
        requireADLogin: true
    }).when("/settings/subs/:subId", {
        controller: "settingsSubDetailsCtrl",
        templateUrl: "/App/Views/SettingsSubDetails.html",
        requireADLogin: true
    }).when("/settings/subs", {
        controller: "settingsSubsCtrl",
        templateUrl: "/App/Views/SettingsSubs.html",
        requireADLogin: true
    }).when("/settings/logs", {
        controller: "settingsLogsCtrl",
        templateUrl: "/App/Views/SettingsLogs.html",
        requireADLogin: true
    }).otherwise({ redirectTo: "/Home" });

    // to help disable the '#' in URLs
    // additional server side configuration may be required
    //$locationProvider.html5Model(true);

    if (scampConfig.settings.clientId === null)
        console.error("Missing $env:APPSETTING_ClientId");
    if (scampConfig.settings.tenantId === null)
        console.error("Missing $env:APPSETTING_TenantId");

    adalProvider.init(
        {
            tenant: scampConfig.settings.tenantId,
            clientId: scampConfig.settings.clientId,
            extraQueryParameter: scampConfig.settings.extraQueryParameter,
            cacheLocation: scampConfig.settings.cacheLocation,
            redirectUri: scampConfig.settings.redirectUri
        },
        $httpProvider
        );
   
}]);
