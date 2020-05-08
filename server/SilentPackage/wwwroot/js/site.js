/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
(function (angular) {
    'use strict';
    var app = angular.module("myApp", []);

    var api = null;


    app.controller('selectAPI', ['$scope', function ($scope) {
        $scope.data = {
            singleSelect: null
        };

        $scope.save = function () {
            api = $scope.data.singleSelect;
            console.log("%cWybrano api: " + api, "font-family:helvetica; font-size:20px; color:white;");
        }

    }]);


    app.controller('userInfo', function ($scope, $http) {
        $scope.master = {};
        $scope.get = function (user) {
            $scope.master = angular.copy(user);
            $http({
                url: 'http://localhost:' + api + '/api/1.1/users/' + $scope.master.id,
                method: 'GET',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137'
                },
            }).then(
                function (response) {
                    $scope.userList = response.data;
                    $scope.status = response.status;
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('usersList', function ($scope, $http) {
        $scope.open = function () {
            $http({
                url: 'http://localhost:' + api + '/api/1.1/users/',
                method: 'GET',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137'
                },
            }).then(
                function (response) {
                    $scope.userList = response.data;
                    status(response.status);
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('createUser', function ($scope, $http) {
        $scope.master = {};
        $scope.create = function (user) {
            $scope.master = angular.copy(user);
            $http({
                url: 'http://localhost:' + api + '/api/1.1/users/' + $scope.master.id,
                method: 'PUT',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137'
                },
            }).then(
                function (response) {
                    $scope.status = response.status;
                    alert("Dodano użytkownika do systemu!");
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('updateDeviceID', function ($scope, $http) {
        $scope.master = {};
        $scope.update = function (user) {
            $scope.master = angular.copy(user);
            $http({
                url: 'http://localhost:' + api + '/api/1.1/users/' + $scope.master.id + '/' + $scope.master.deviceid,
                method: 'POST',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137'
                },
            }).then(
                function (response) {
                    $scope.status = response.status;
                    alert("Został dodany identyfiaktor sprzętowy");
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('deleteUser', function ($scope, $http) {
        $scope.master = {};
        $scope.delete = function (user) {
            $scope.master = angular.copy(user);
            $http({
                url: 'http://localhost:' + api + '/api/1.1/users/' + $scope.master.id,
                method: 'DELETE',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137'
                },
            }).then(
                function (response) {
                    $scope.status = response.status;
                    alert("Użytkownik został usunięty!");
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('reportInfo', function ($scope, $http) {
        $scope.master = {};
        $scope.get = function (user) {
            $scope.master = angular.copy(user);
            $http({
                url: 'http://localhost:' + api + '/api/1.1/reports/' + $scope.master.id,
                method: 'GET',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137',
                    'timestamp': 'true'
                },
            }).then(
                function (response) {
                    $scope.url = window.location.protocol + '//' + window.location.hostname + ':' + location.port + '/api/1.1/reports/';
                    $scope.reports = response.data;
                    $scope.status = response.status;
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('reportList', function ($scope, $http) {
        $scope.open = function (reports) {
            $scope.master = angular.copy(reports);
            var startDate = (Date.parse($scope.master.startrange) / 1000) + 7200 ;
            var endDate = (Date.parse($scope.master.endrange) / 1000) + 7200 ;
            $http({
                url: 'http://localhost:' + api + '/api/1.1/reports/' + $scope.master.id + '/' + startDate + '/' + endDate,
                method: 'GET',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137'
                }
            }).then(
                function (response) {
                    $scope.url = window.location.protocol + '//' + window.location.hostname + ':' + location.port + '/api/1.1/reports/';
                    $scope.reportsList = response.data;
                    status(response.status);
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('deleteReport', function ($scope, $http) {
        $scope.master = {};
        $scope.open = function (report) {
            $scope.master = angular.copy(report);
            $http({
                url: 'http://localhost:' + api + '/api/1.1/reports/' + $scope.master.deviceid + '/' + $scope.master.filename,
                method: 'DELETE',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137',

                },
            }).then(
                function (response) {
                    $scope.status = response.status;
                    alert("Usunięto raport!");
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('deleteAllReport', function ($scope, $http) {
        $scope.master = {};
        $scope.open = function (report) {
            $scope.master = angular.copy(report);
            $http({
                url: 'http://localhost:' + api + '/api/1.1/reports/' + $scope.master.id,
                method: 'DELETE',
                contentType: 'application/json',
                headers: {
                    'API': 'abc2137',

                },
            }).then(
                function (response) {
                    $scope.status = response.status;
                    alert("Usunięto raporty!");
                }, function (response) {
                    status(response.status);
                }
            );
        };
    });

    app.controller('TimestampCtrl', ['$scope', function ($scope) {
        $scope.toTimestamp = function (date) {
            return new Date(date * 1000);
        };
    }]);
})(window.angular);

function status(number) {
    switch (number) {
        case 404:
            alert('Brak danych!');
            console.log("%cBrak danych [404]", "font-family:helvetica; font-size:50px; font-weight:bold; color:red; -webkit-text-stroke:1px white;");
            break;
        case 403:
            alert('Brak dostępu!');
            console.log("%cBrak dostępu! [403]", "font-family:helvetica; font-size:50px; font-weight:bold; color:red; -webkit-text-stroke:1px white;");
            break;
        case 409:
            alert('Konflikt danych!');
            console.log("%cKonflikt danych! [409]", "font-family:helvetica; font-size:50px; font-weight:bold; color:red; -webkit-text-stroke:1px white;");
            break;
        case 500:
            alert('Błąd serwera!');
            console.log("%cBład serwera [500]", "font-family:helvetica; font-size:50px; font-weight:bold; color:red; -webkit-text-stroke:1px white;");
            break;
        case 400:
            alert('Złe zapytanie!');
            console.log("%cZłe zapytanie! [400]", "font-family:helvetica; font-size:50px; font-weight:bold; color:red; -webkit-text-stroke:1px white;");
            break;
        case 416:
            alert('Dane spoza zakresu!');
            console.log("%cDane spoza zakresu [416]", "font-family:helvetica; font-size:50px; font-weight:bold; color:red; -webkit-text-stroke:1px white;");
            break;
    }
}