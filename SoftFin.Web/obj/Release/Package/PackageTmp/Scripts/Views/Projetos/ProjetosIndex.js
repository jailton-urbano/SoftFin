$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});
var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination']);
app.directive('datetimez', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {

            element.datetimepicker({
                dateFormat: 'dd/MM/yyyy',
                language: 'ptbr',
                pickTime: false,
                startDate: '01-11-2013',      // set a minimum date
                endDate: '01-11-2030'          // set a maximum date
            }).on('changeDate', function (e) {
                ngModelCtrl.$setViewValue(moment(e.localDate).format());
                scope.$apply();
                $(this).datetimepicker('hide');
            });
        }
    };
});

app.filter('yesNo', function () {
    return function (input) {
        return input === "True" ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../Projetos/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });


        $scope.Lista = [];
        $scope.entidade = '';
        $scope.ModoConsulta = false;
        $scope.showMostraTodos = true;
        $scope.procurar = "";
        $scope.showCancelar = false;
        $scope.showCadastro = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.showGrid = true;
        $scope.showloadGrid = false;
        


        $scope.Ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
              

        $scope.Novo = function () {
            $scope.ModoConsulta = false;

            $scope.LoadporId(0);
        }

        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadporId(item.id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadporId(item.id);
        }
        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.showNOK = false;
            $scope.showOK = false;
            $scope.LoadTodos(); 
        }



        $scope.MsgError = function (msg) {
            $scope.msgNOK = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        }

        $scope.MsgSucesso = function (msg) {
            $scope.msgOK = msg;
            $scope.showOK = true;
            $scope.showNOK = false;
        }


        $scope.Pesquisa = function (msg) {
            $scope.LoadTodos();
        }


        $scope.Excel = function (msg) {
            window.location = '../../Projetos/Excel';
        }


        $scope.Ficha = function (item) {
            console.log(item);
            window.location = '../../Projetos/pdf/' + item.id;
        }

        $scope.Salvar = function () {

            Metronic.blockUI({
                boxed: true
            });


            var postdata = $scope.entidade;

            $http.post("../../Projetos/Salvar", postdata)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus === "OK") {
                        $scope.showOK = true;
                        $scope.msgOK = data.DSMessage;
                    } else {
                        $scope.showNOK = true;
                        $scope.Erros = data.Erros;
                        $scope.msgNOK = data.DSMessage;
                    }

                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;

                        }, 18000);



                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";
                });
        }

        $scope.Cancel = function () {
            $scope.showImportacao = false;
            $scope.showMostraTodos = true;
            $location.hash('DivTotal');
        };

        $scope.LoadProjetos = function () {
            $http.post("../../Projetos/ObterProjetos")
                .success(function (data) {
                    $scope.ListaProjetosDrop = data;
                });
        };
        $scope.LoadProjetos();

        $scope.LoadContratos = function () {
            $http.post("../../Projetos/ObterContratos")
                .success(function (data) {
                    $scope.ListaContratosDrop = data;
                });
        };
        $scope.LoadContratos();

        $scope.LoadContratosItem = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Projetos/ObterContratosItem", {'id': $scope.entidade.ContratoId})
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaContratosItemDrop = data;
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        };

        $scope.LoadCategoriaProfissional = function () {
            $http.post("../../Projetos/ObterCategoriaProfissional")
                .success(function (data) {
                    $scope.ListaCategoriaProfissionalDrop = data;
                });
        };
        $scope.LoadCategoriaProfissional();


        $scope.LoadTodos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Projetos/ObterTodos/")
                .success(function (data) {
                    $scope.Lista = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }

        $scope.LoadTodos();
        $scope.LoadporId = function (id) {
            $http.get("../../Projetos/ObterPorId/" + id)
                .success(function (data) {
                    $scope.entidade = data;
                    $scope.LoadContratosItem();
                    $scope.showGrid = false;
                    $scope.showManut = true;
                    

                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                });
        }


        $scope.Marca= function (item) {
            item.selecionado = !item.selecionado;
        }

        $scope.Excluir = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluir.html',
                controller: ControllerExcluir,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.LoadTodos();
            }, function () {
            });
        }


    }
]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {




    $scope.item = item.item;

    $scope.Ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Projetos/Excluir", $scope.item )
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus === "OK") {
                    $modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;
                    $scope.Erros = data.Erros;
                }
                $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;

                    }, 8000);
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
