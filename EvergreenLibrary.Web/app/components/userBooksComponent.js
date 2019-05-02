app.component('userBooks', {
    templateUrl: "/app/views/userBooks.html",
    controller: "userBooksController as ctrl",
    bindings: {
        userId: '<'
    }
  });