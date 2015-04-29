'use strict';
angular.module('scamp')
.factory('settingsSubSetupSvc', ['$http', '$q', function ($http, $q) {

    return {

        // gets specified subscription details
        getSub: function (subId) {
            return {
                subname: "Primary Subscription",
                subID: subId,
                subAdmin: "johndoe@hotmail.com",
                LastChgd: "10/10/2015",
                svcLimits : [
                    {
                        type: "cloud service",
                        current: 20,
                        maximum: 25,
                        msg: "recommend quota increase"
                    },
                    {
                        type: "storage accounts",
                        current: 10,
                        maximum: 100
                    },
                    {
                        type: "resource groups",
                        current: 40,
                        maximum: 500
                    },
                    {
                        type: "compute cores",
                        current: 40,
                        maximum: 350
                    },
                    {
                        type: "web hosting plans",
                        current: 40,
                        maximum: 5000
                    }

                ],
                events : [
                    {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "Bob Smith",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "Jane Doe",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }
                ]
            };
        },

        getAvailableRegions: function () {
            var list = [
                    {
                        Name: "Central US - Iowa",
                        checked: true
                    },
                    {
                        Name: "East US - Virginia",
                        checked: true
                    },
                    {
                        Name: "East US 2 - Virginia",
                        checked: true
                    },
                    {
                        Name: "North Central US - Illinois",
                        checked: true
                    },
                    {
                        Name: "North Europe - Ireland",
                        checked: false
                    },
                    {
                        Name: "East Asia - Hong Kong",
                        checked: false
                    },
                    {
                        Name: "Japan East - Saitama Prefecture",
                        checked: true
                    },
                    {
                        Name: "Brazil South - Sao Paulo State",
                        checked: true
                    },
                    {
                        Name: "Australia East - New South Wales",
                        checked: false
                    }
            ];

            return list;
        },

        getGalleryImages: function () {
        var list = [
                {
                    Name: "Windows Server 2012 R2 Datacenter",
                    checked: true
                },
                {
                    Name: "Windows Server Essentials Experience",
                    checked: true
                },
                {
                    Name: "SQL Server 2014 Enterprise Optimized for DataWarehousing Workloads",
                    checked: true
                },
                {
                    Name: "Ubuntu Server 12.04 LTS",
                    checked: true
                },
                {
                    Name: "CoreOS Stable",
                    checked: false
                },
                {
                    Name: "OpenLogic 7.0",
                    checked: false
                },
                {
                    Name: "openSUSE 13.1",
                    checked: true
                },
                {
                    Name: "Oracle Database 12c Enterprise Edition",
                    checked: true
                },
                {
                    Name: "Oracle WebLogic Server 11g Enterprise Edition",
                    checked: false
                }
        ];

        return list;
    }


    };
}]);