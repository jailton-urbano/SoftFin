$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return '';
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

var app = angular.
    module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination']);

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
        return input == "True" ? 'Sim' : 'Não';
    };
});


app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal',
    function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {
        $scope.ListaNotas = [];
        $scope.idSelecionado = 0;
        $scope.showMostraTodos = true;
        $scope.procurar = getParameterByName("Pesquisar");
        $scope.showCancelar = false;
        $scope.showCadastro = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.showGrid = true;
        $scope.showloadGrid = false;
        $scope.ModoConsulta = false;

        $scope.EditAccess = false;
        $http.post("../../BM/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

        
        $scope.filtro2 = {
            banco_id: "",
            dataIni: moment().add(-8, 'days').format(),
            dataFim: moment().add(8, 'days').format(),
            valorIni: 0,
            valorFim: 9999999.99

        };




        $scope.getTotal = function () {
            var total = 0;
            if ($scope.lista !== undefined)
                for (var i = 0; i < $scope.lista.length; i++) {
                    var valor = $scope.lista[i].valor;
                    total += valor;
                }
            return total;
        };


        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

        $scope.bm = [];

        $scope.Novo = function () {
            $scope.ModoConsulta = false;

            $scope.LoadBMporId(0);
        }

        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.Pesquisar();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadBMporId(item.id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadBMporId(item.id);
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


        $scope.Pesquisar = function (msg) {
            $scope.LoadBM();
        }


        $scope.Excel = function (msg) {
            window.location = '../../BM/Excel';
        }

        $scope.Salvar = function () {



            $scope.opts = {
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalSalvar.html',
                controller: ControllerSalvar,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: $scope.bm }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;
                        $scope.msgSalvar = "Salvar";
                        $scope.btnSalvar = true;

                    }, 18000);
            }, function () {
            });


        }

        $scope.cancel = function () {
            $scope.showImportacao = false;
            $scope.showMostraTodos = true;
            $location.hash('DivTotal');
        };

        //$scope.LoadListaUnidadeNegocios = function () {
        //    $http.post("../../BM/ObterUnidadeNegocios")
        //        .success(function (data) {
        //            $scope.ListaUnidadeNegocios = data;
        //        });
        //};
        //$scope.LoadListaUnidadeNegocios();



        $scope.LoadListaBanco = function () {
            $http.post("../../BM/ObterBanco")
                .success(function (data) {
                    $scope.ListaBancos = data;
                });
        };
        $scope.LoadListaBanco();

        $scope.LoadListaIndicadorMovimento = function () {
            $http.post("../../BM/ObterIndicadorMovimento")
                .success(function (data) {
                    $scope.ListaIndicadorMovimentos = data;
                });
        };
        $scope.LoadListaIndicadorMovimento();

        $scope.LoadListaPlanoDeContas = function () {
            $http.post("../../BM/ObterPlanoDeContas")
                .success(function (data) {
                    $scope.ListaPlanoDeContas = data;
                });
        };
        $scope.LoadListaPlanoDeContas();

        $scope.LoadListaTipoMovimento = function () {
            $http.post("../../BM/ObterTipoMovimento")
                .success(function (data) {
                    $scope.ListaTipoMovimentos = data;
                });
        };
        $scope.LoadListaTipoMovimento();

        $scope.LoadListaOrigemMovimento = function () {
            $http.post("../../BM/ObterOrigemMovimento")
                .success(function (data) {
                    $scope.ListaOrigemMovimentos = data;
                });
        };
        $scope.LoadListaOrigemMovimento();

        $scope.LoadListaTipoDocumento = function () {
            $http.post("../../BM/ObterTipoDocumento")
                .success(function (data) {
                    $scope.ListaTipoDocumentos = data;
                });
        };
        $scope.LoadListaTipoDocumento();

        $scope.LoadListaUnidades = function () {
            $http.post("../../CG/ObterUnidades")
                .success(function (data) {
                    $scope.ListaUnidades = data;
                });
        };
        $scope.LoadListaUnidades();



        $scope.LoadBM = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../BM/ObterTodos/", $scope.filtro2)
            .success(function (data) {
                $scope.lista = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }

        
        var dataParam = getParameterByName("DataInicial");
        
        if (dataParam != "") {
            $scope.filtro2.dataIni = dataParam;
            $scope.filtro2.dataFim = dataParam;
            $scope.LoadBM();
        }

        $scope.LoadBMporId = function (id) {
            $scope.cpagconsulta = "";
            $scope.nfconsulta = "";
            $scope.razaoconsulta = "";

            $http.get("../../BM/ObterPorId/" + id)
            .success(function (data) {
                $scope.bm = data;
                $scope.showGrid = false;
                $scope.showManut = true;


                $http.get("../../BM/DetalheBM/" + id)
                    .success(function (data) {
                        $scope.cpagconsulta = data.cpagconsulta;
                        $scope.nfconsulta = data.nfconsulta;
                        $scope.razaoconsulta = data.razaoconsulta;





                    }).error(function (data) {
                       // $scope.MsgError("Sistema indisponivel");
                    });


            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
            });
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
                $scope.LoadBM();
            }, function () {
            });
        }


    }
]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {



    $scope.item = item.item;

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../BM/Excluir", { id: $scope.item.id })
            .success(function (data) {
                Metronic.unblockUI();
            if (data.CDStatus == "OK") {
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

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}



var ControllerSalvar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.entidade = item.item;
    $scope.mensagem = "";

    Metronic.blockUI({
        boxed: true
    });

    $http.post("../../BM/verificaDuplicidade", $scope.entidade)
        .success(function (data) {
            Metronic.unblockUI();
            if (data.CDStatus == "NOK") {
                $scope.mensagem = data.DSMessage;
            } 
        }).error(function (data) {
            Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });


    $scope.ok = function () {
        $http.post("../../BM/Salvar", $scope.entidade )
        .success(function (data) {
            if (data.CDStatus == "OK") {
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
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}