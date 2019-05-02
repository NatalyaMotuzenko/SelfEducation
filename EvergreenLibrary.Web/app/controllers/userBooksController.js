'use strict';
app.controller('userBooksController', ['$scope', '$element', '$timeout', 'userBooksService', 'localStorageService', function ($scope, $element, $timeout, userBooksService,  localStorageService) {

    $scope.userBooks = [];

    $scope.userName = localStorageService.get('authorizationData') ? localStorageService.get('authorizationData').userName : "";

    $scope.message = "";

        userBooksService.getUserBooks(this.userId).then(function (results) {

            $scope.userBooks = results.data;

        }, function (error) {
            $scope.message = error.data.message;
        });

    $scope.readBook = function (id) {
        var bookId = $scope.userBooks[id].id;

        userBooksService.readTakeBook(bookId, true).then(readTakeBookSuccess, readTakeBookError);
        function readTakeBookSuccess(response) {
            $scope.userBooks[id].state = $scope.userName;
        }
        function readTakeBookError(err) {
            $scope.message = err.data.error_description === undefined ? err.data.message : err.data.error_description;
        }
    };

    $scope.returnBook = function (id) {
        var bookId = $scope.userBooks[id].id;

        userBooksService.readTakeBook(bookId, false).then(readTakeBookSuccess, readTakeBookError);
        function readTakeBookSuccess(response) {
            $scope.userBooks[id].state = "free";
        }
        function readTakeBookError(err) {
            $scope.message = err.data.error_description === undefined ? err.data.message : err.data.error_description;
        }
    };

}]);