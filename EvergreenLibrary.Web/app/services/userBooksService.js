'use strict';
app.factory('userBooksService', ['$http', function ($http) {

    var serviceBase = 'http://localhost:59822/';
    var userBooksServiceFactory = {};

    var _readTakeBook = function (bookId, take) {

        return $http.put(serviceBase + 'api/users/books/' + bookId + '?take=' + take).then(function (responce) {
            return responce;
        });
    };

    var _getUserBooks = function (userId) {
        if (!userId)
            userId = "";
        return $http.get(serviceBase + 'api/users/books/' + userId).then(function (result) {
            return result;
        });
    };

    userBooksServiceFactory.readTakeBook = _readTakeBook;
    userBooksServiceFactory.getUserBooks = _getUserBooks;

    return userBooksServiceFactory;

}]);