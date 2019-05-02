'use strict';
app.factory('userService', ['$http', function ($http) {

    var serviceBase = 'http://localhost:59822/';
    var userServiceFactory = {};

    var _getUsers = function () {

        return $http.get(serviceBase + 'api/accounts/users').then(function (result) {
            return result;
        });
    };

    var _getUserByName = function (userName) {

        return $http.get(serviceBase + 'api/accounts/user/' + userName).then(function (result) {
            return result;
        });
    };

    var _getUserById = function (userId) {

        return $http.get(serviceBase + 'api/accounts/user/' + userId).then(function (result) {
            return result;
        });
    };

    var _addUser = function (user) {

        return $http.post(serviceBase + 'api/accounts', user).then(function (responce) {
            return responce;
        });
    };

    var _deleteUser = function (userId) {

        return $http.delete(serviceBase + 'api/accounts/user/' + userId).then(function (responce) {
            return responce;
        });
    };


    var _editUser = function (bookId, book) {

        return $http.put(serviceBase + 'api/books/' + bookId, book).then(function (responce) {
            return responce;
        });
    };

    userServiceFactory.getUserByName = _getUserByName;
    userServiceFactory.getUsers = _getUsers;
    userServiceFactory.addUser = _addUser;
    userServiceFactory.deleteUser = _deleteUser;
    userServiceFactory.getUserById = _getUserById;

    return userServiceFactory;

}]);