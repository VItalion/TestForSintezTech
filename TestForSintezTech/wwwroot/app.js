(function () {
    'use strict';

    // нужно определять каждый контроллер в отдельном файле, настроить бандлинг и минификацию. Но я не успеваю это сделать

    var tokenKey = "accessToken";

    var app = angular.module('viewApp', ['ngRoute', 'ngCookies'])
        .service('authInterceptor', function ($q) {
            var service = this;
            service.responseError = function (response) {
                if (response.status !== 200) {
                    var label = document.getElementById('infoLabel');
                    label.innerHTML = response.statusText;
                }
                if (response.status === 401) {
                    location.assign("/");
                }
                return $q.reject(response);
            };
        })
        .config(["$httpProvider", "$routeProvider", function ($httpProvider, $routeProvider) {
            $routeProvider.when('/', {
                templateUrl: 'login.html',
                controller: 'authorizationController'
            });
            $routeProvider.when('/register', {
                templateUrl: 'register.html',
                controller: 'authorizationController'
            });
            $routeProvider.when('/view', {
                templateUrl: 'generalView.html',
                controller: 'subdivisionViewController'
            });
            $routeProvider.when('/subdivision', {
                templateUrl: 'subdivisionsView.html',
                controller: 'subdivisionController'
            });
            $routeProvider.when('/createSubdivision', {
                templateUrl: 'createSubdivision.html',
                controller: 'createController'
            });
            $routeProvider.when('/editSubdivision', {
                templateUrl: 'editSubdivision.html',
                controller: 'editController'
            });
            $routeProvider.when('/employees', {
                templateUrl: 'emoloyees.html',
                controller: 'employeesController'
            });
            $routeProvider.when('/createEmployee', {
                templateUrl: 'createEmployee.html',
                controller: 'createController'
            });
            $routeProvider.when('/selectEmployee', {
                templateUrl: 'selectEmployee.html',
                controller: 'selectController'
            });
            $routeProvider.when('/editEmployee', {
                templateUrl: 'editEmployee.html',
                controller: 'editController'
            });

            $httpProvider.interceptors.push('authInterceptor');
        }]);

    app.factory('subdivisionIdService', subdivisionIdService)
        .factory('employeeService', employeeService)
        .controller('authorizationController', authorizationController)
        .controller('subdivisionController', ['$scope', '$http', 'subdivisionIdService', subdivisionController])
        .controller('subdivisionViewController', ['$scope', '$http', 'subdivisionIdService', 'employeeService', subdivisionViewController])
        .controller('employeesController', ['$scope', '$http', 'subdivisionIdService', 'employeeService', employeesController])
        .controller('editController', ['$scope', '$http', 'subdivisionIdService', 'employeeService', editController])
        .controller('selectController', ['$scope', '$http', 'subdivisionIdService', 'employeeService', selectController])
        .controller('createController', ['$scope', '$http', 'subdivisionIdService', 'employeeService', createController]);


    function subdivisionIdService($cookieStore) {
        return {
            setId: function (id) {
                $cookieStore.put('currentId', id);
            },
            getId: function () {
                return $cookieStore.get('currentId');
            }
        }
    }

    function employeeService($cookieStore) {
        return {
            setEmployee: function (employee) {
                $cookieStore.put('employee.FirstName', employee.FirstName);
                $cookieStore.put('employee.LastName', employee.LastName);
                $cookieStore.put('employee.Patronymic', employee.Patronymic);
                $cookieStore.put('employee.Age', employee.Age);
                $cookieStore.put('employee.Gender', employee.Gender);
                $cookieStore.put('employee.Position', employee.Position);
                $cookieStore.put('employee.Id', employee.Id);
            },
            getEmployee: function () {
                return {
                    Id: $cookieStore.get('employee.Id'),
                    FirstName: $cookieStore.get('employee.FirstName'),
                    LastName: $cookieStore.get('employee.LastName'),
                    Patronymic: $cookieStore.get('employee.Patronymic'),
                    Age: $cookieStore.get('employee.Age'),
                    Gender: $cookieStore.get('employee.Gender'),
                    Position: $cookieStore.get('employee.Position')
                };
            }
        }
    }

    function authorizationController($scope) {
        $scope.title = 'authorizationController';

        activate();

        function activate() {
            sessionStorage.removeItem(tokenKey);
            var label = document.getElementById('infoLabel');
            label.innerHTML = '';
        }

        $scope.login =
            function login(user, loginForm) {
                var button = document.getElementById("sumbitButton");

                if (loginForm.$valid && !button.classList.contains("disabled")) {
                    var parameter = JSON.stringify({
                        Login: user.Login,
                        Password: user.Password
                    });
                    
                    button.classList.add("disabled");

                    $.ajax({
                        url: '/token',
                        type: 'POST',
                        contentType: 'application/json',
                        data: parameter,
                        success: function (token) {
                            sessionStorage.setItem(tokenKey, token);
                            location.assign("#!subdivision");
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            var label = document.getElementById('infoLabel');
                            label.innerHTML = XMLHttpRequest.responseText;
                            button.classList.remove("disabled");
                        }
                    });
                }
            }
        $scope.register =
            function register(user, regiserForm) {
                if (regiserForm.$valid && user.Password === user.ConfirmPassword) {
                    var parameter = JSON.stringify({
                        Login: user.Login,
                        Password: user.Password
                    });
                    $.ajax({
                        url: '/register',
                        type: 'POST',
                        contentType: 'application/json',
                        data: parameter,
                        success: function (result) {
                            var label = document.getElementById('infoLabel');
                            label.innerHTML = "Пользователь зарегистрирован";
                            location.assign("/");
                        }
                    });
                } else {
                    var label = document.getElementById('infoLabel');
                    label.innerHTML = "Введенные пароли не совпадают.";
                }
            }
    }

    function employeesController($scope, $http, subdivisionIdService, employeeService) {
        $scope.title = 'employeesController';

        var subdivisionId = subdivisionIdService.getId();
        var name = JSON.stringify({
            subdivision: subdivisionId
        });

        activate();

        function activate() {
            var token = sessionStorage.getItem(tokenKey);
            $http.defaults.headers.common.Authorization = 'Bearer ' + token;

            var label = document.getElementById('infoLabel');
            label.innerHTML = '';

            $http({
                method: 'GET',
                url: '/api/employees',
                contentType: 'application/json',
                params: { subdivision: subdivisionId }
            }).then(function successCallback(responce) {
                $scope.employees = responce.data
            }, function errorCallback(responce) {

            });
        }

        $scope.select = function select(employee) {
            employeeService.setEmployee(employee);
            location.assign("#!editEmployee");
        }

        $scope.delete =
            function remove(employee) {
                var employeeJson = JSON.stringify({
                    id: employee.Id
                });

                $.ajax({
                    url: '/api/employees',
                    type: 'DELETE',
                    contentType: 'application/json',
                    data: employeeJson,
                    success: function (result) {
                        var label = document.getElementById('infoLabel');
                        label.innerHTML = "Пользователь зарегистрирован";
                    }
                });
            }

        $scope.toEdit =
            function toEdit(employee) {
                employeeService.setEmployee(employee);
                location.assign("#!editEmployee");
            }
    }

    function subdivisionController($scope, $http, subdivisionIdService) {
        $scope.title = 'subdivisionController';

        activate();

        function activate() {
            var token = sessionStorage.getItem(tokenKey);
            $http.defaults.headers.common.Authorization = 'Bearer ' + token;

            var label = document.getElementById('infoLabel');
            label.innerHTML = '';

            $http({
                method: 'GET',
                url: '/api/subdivisions',
                contentType: 'application/json'
            }).then(function successCallback(responce) {
                $scope.subdivisions = responce.data;
            }, function errorCallback(responce) {
                //alert(responce.statusText);
            });
        }

        $scope.open =
            function open(subdivision) {
                subdivisionIdService.setId(subdivision.Id);
                location.assign("#!view");
            }

        $scope.delete =
            function remove(subdivision) {
                var parameter = JSON.stringify({
                    id: subdivision.Id
                });
                $http({
                    method: 'DELETE',
                    url: '/api/subdivisions/' + subdivision.Id,
                    contentType: 'application/json',
                    data: parameter
                }).then(function successCallback(responce) {
                    location.assign("#!subdivision");
                }, function errorCallback(responce) {
                    location.assign("#!subdivision");
                });
            }

        $scope.toEdit = function (subdivisionId) {
            subdivisionIdService.setId(subdivisionId);
            location.assign("#!editSubdivision");
        }
    }

    function subdivisionViewController($scope, $http, subdivisionIdService, employeeService) {
        activate();

        function activate() {
            var token = sessionStorage.getItem(tokenKey);
            $http.defaults.headers.common.Authorization = 'Bearer ' + token;

            var label = document.getElementById('infoLabel');
            label.innerHTML = '';

            $http({
                method: 'GET',
                url: '/api/subdivisions/' + subdivisionIdService.getId(),
                contentType: 'application/json'
            }).then(function successCallback(responce) {
                $scope.subdivisions = responce.data.Children;
            }, function errorCallback(responce) {
            });

            $http({
                method: 'GET',
                url: '/api/Employees/' + subdivisionIdService.getId(),
                contentType: 'application/json'
            }).then(function successCallback(responce) {
                $scope.employees = responce.data
            }, function errorCallback(responce) {

            });
        }

        $scope.open =
            function open(subdivision) {
                subdivisionIdService.setId(subdivision.Id);
                location.assign("#!view");
            }

        $scope.deleteSubdivision =
            function remove(subdivision) {
                var parameter = JSON.stringify({
                    id: subdivision.Id
                });
                $http({
                    method: 'DELETE',
                    url: '/api/subdivisions/' + subdivision.Id,
                    contentType: 'application/json',
                    data: parameter
                }).then(function successCallback(responce) {
                    location.assign("#!view");
                }, function errorCallback(responce) {
                    location.assign("#!view");
                });
            }

        $scope.toEditSubdivision =
            function (subdivisionId) {
                subdivisionIdService.setId(subdivisionId);
                location.assign("#!editSubdivision");
            }

        $scope.deleteEmployee = function remove(employee) {
            $http({
                method: 'DELETE',
                url: '/api/employees/employeeId=' + employee.Id + '&subdivisionId=' + subdivisionIdService.getId(),
                contentType: 'application/json'
            }).then(function successCallback(responce) {
                location.assign("#!view");
            }, function errorCallback(responce) {
                location.assign("#!view");
            });
        }

        $scope.toEditEmployee = function toEdit(employee) {
            employeeService.setEmployee(employee);
            location.assign("#!editEmployee");
        }
    }

    function editController($scope, $http, subdivisionIdService, employeeService) {

        activate();

        function activate() {
            var token = sessionStorage.getItem(tokenKey);
            $http.defaults.headers.common.Authorization = 'Bearer ' + token;

            var id = subdivisionIdService.getId();
            $scope.subdivision = {
                Id: id
            };

            $scope.employee = employeeService.getEmployee();
        }

        $scope.editSubdivision =
            function editSubdivision(subdivision, subdivisionForm) {
                var parameter = JSON.stringify({
                    id: subdivision.Id,
                    Name: subdivision.Name
                });

                var label = document.getElementById('infoLabel');
                label.innerHTML = '';

                $http({
                    method: 'PUT',
                    url: '/api/subdivisions/' + subdivision.Id,
                    contentType: 'application/json',
                    data: parameter
                }).then(function successCallback(responce) {
                    location.assign("#!subdivision");
                }, function errorCallback(responce) {
                    location.assign("#!subdivision");
                });
            }

        $scope.editEmployee =
            function editEmployee(employee) {
                var employeeJson = JSON.stringify({
                    Id: employee.Id,
                    FirstName: employee.FirstName,
                    LastName: employee.LastName,
                    Patronymic: employee.Patronymic,
                    Age: employee.Age,
                    Gender: employee.Gender,
                    Position: employee.Position,
                    Subdivision: subdivisionIdService.getId()
                });


                $http({
                    method: 'PUT',
                    url: '/api/employees/' + employee.Id,
                    contentType: 'application/json',
                    data: employeeJson
                }).then(function successCallback(responce) {
                    location.assign("#!view");
                }, function errorCallback(responce) {

                });
            }
    }

    function createController($scope, $http, subdivisionIdService, employeeService) {
        activate();

        function activate() {
            var token = sessionStorage.getItem(tokenKey);
            $http.defaults.headers.common.Authorization = 'Bearer ' + token;

            var label = document.getElementById('infoLabel');
            label.innerHTML = '';
        }

        $scope.createSubdivision =
            function create(subdivision, subdivisionForm) {
                if (subdivisionForm.$valid) {

                    var parameter = JSON.stringify({
                        Name: subdivision.Name,
                        Parent: {
                            Id: subdivisionIdService.getId()
                        }
                    });
                    $http({
                        method: 'POST',
                        url: '/api/subdivisions/',
                        contentType: 'application/json',
                        data: parameter
                    }).then(function successCallback(responce) {
                        location.assign("#!view");
                    }, function errorCallback(responce) {
                        location.assign("#!view");
                    });
                }
            }

        $scope.createEmployee =
            function create(employee, employeeForm) {
                if (employeeForm.$valid) {
                    var id = subdivisionIdService.getId();
                    var employeeJson = JSON.stringify({
                        FirstName: employee.FirstName,
                        LastName: employee.LastName,
                        Patronymic: employee.Patronymic,
                        Age: employee.Age,
                        Gender: employee.Gender,
                        Position: employee.Position,
                        Subdivision: id
                    });

                    $http({
                        method: 'POST',
                        url: '/api/employees',
                        contentType: 'application/json',
                        data: employeeJson
                    }).then(function successCallback(responce) {
                        location.assign("#!view");
                    }, function errorCallback(responce) {

                    });
                }
            }
    }

    function selectController($scope, $http, subdivisionIdService, employeeService) {
        activate();

        function activate() {
            var token = sessionStorage.getItem(tokenKey);
            $http.defaults.headers.common.Authorization = 'Bearer ' + token;

            var label = document.getElementById('infoLabel');
            label.innerHTML = '';

            $http({
                method: 'GET',
                url: '/api/employees/GetAll',
                contentType: 'application/json',
                params: { subdivision: subdivisionIdService.getId() }
            }).then(function successCallback(responce) {
                $scope.employees = responce.data
            }, function errorCallback(responce) {

            });
        }
    }
})();