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
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

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
        return input == true ? 'Sim' : 'Não';
    }
});

app.filter('limpazero', function () {
    return function (input) {
        return input == 0 ? '' : input;
    };
});

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal',
    function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../Boleto/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");           
            });
       

        $scope.ListaNotas = [];
        $scope.idSelecionado = 0
        $scope.showMostraTodos = true;
        $scope.procurar = getParameterByName("Pesquisa");
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

        $scope.Historico = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalHistorico.html',
                controller: ControllerHistorico,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: 0 }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {

            }, function () {
            });
        }

        
        $scope.filtro = {
            TipoVinculo: 1,
            dataIni: moment().add(-8, 'days').format(),
            dataFim: moment().add(8, 'days').format()
        };

        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };

        $scope.LoadListaBanco = function () {
            $http.post("../../Boleto/ObterBancoCartao")
                .success(function (data) {
                    $scope.ListaBancos = data;
                });
        };
        


        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        $scope.ListaTipoBoleto = [
            { Value: 1, Text: "Não Emitido" },
            { Value: 3, Text: "Emitido" }
        ];
 
        $scope.Novo = function () {
            $scope.ModoConsulta = false;
            $scope.LoadporId(0);

        }
        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.Pesquisa();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadporId(item.id);
        }
        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadporId(item.id);
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
        $scope.GerarBoleto = function (item) {

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: true,
                keyboard: true,
                templateUrl: 'modalSalvar.html',
                controller: ControllerSalvar,
                resolve: {}
            };

            $scope.opts.resolve.item = function () {
                return item;
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.Pesquisa();
            }, function () {
            });




        };

        $scope.EnvioPorEmail = function (item) {

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: true,
                keyboard: true,
                templateUrl: 'modalEmail.html',
                controller: ControllerEmail,
                resolve: {}
            };

            $scope.opts.resolve.item = function () {
                return item;
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.Pesquisa();
            }, function () {
                $scope.Pesquisa();
            });

            


        }


        $scope.CancelarBoleto = function (item) {

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: true,
                keyboard: true,
                templateUrl: 'modalCancelar.html',
                controller: ControllerCancelar,
                resolve: {}
            };

            $scope.opts.resolve.item = function () {
                return item;
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.Pesquisa();
            }, function () {
                $scope.Pesquisa();
            });

            


        }

        $scope.GerarArquivoCNAB = function () {

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: true,
                keyboard: true,
                templateUrl: 'modalGerarArquivoCNAB.html',
                controller: ControllerGerarArquivoCNAB,
                resolve: {}
            };

            $scope.opts.resolve.item = function () {
                return 0;
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.Pesquisa();
            }, function () {
                $scope.Pesquisa();
            });




        }

        $scope.Pesquisa = function () {
            Metronic.blockUI({
                boxed: true
            });
            
            $http.post("../../Boleto/ObterTodos/", $scope.filtro)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus === "OK")
                        $scope.ListaBoleto = data.objs;
                    else
                        $scope.MsgSucesso("Não retornou dados");

                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;
                        }, 18000);

            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }
        $scope.LoadPorId = function (id) {
            $scope.cpagconsulta = "";
            $scope.nfconsulta = "";
            $scope.razaoconsulta = "";

            $scope.LoadListaBanco();

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
        };
    }
]);


var ControllerSalvar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item;
    $scope.showBTNOK = true;

    Metronic.blockUI({
        boxed: true
    });

    $scope.LoadListaBanco = function () {
        $http.post("../../Boleto/ObterBanco")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaBancos = data;
            });
    };
    $scope.LoadListaBanco();


    $scope.LoadBanco = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Boleto/Boleto", { idbanco: item.idbanco })
            .success(function (data) {
                Metronic.unblockUI();
                $scope.item.nossoNumero = data.nossoNumero;
                $scope.item.numeroDocumento = data.numBoleto;
                $scope.item.carteira = data.carteira;
                $scope.item.TextoBoleto01 = data.TextoBoleto01;
                $scope.item.TextoBoleto02 = data.TextoBoleto02;
                $scope.item.TextoBoleto03 = data.TextoBoleto03;
                $scope.item.codigoBanco = data.codigoBanco;
                $scope.item.agencia = data.agencia;
                $scope.item.agenciaDigito = data.agenciaDigito;
                $scope.item.contaCorrente = data.contaCorrente;
                $scope.item.contaCorrenteDigito = data.contaCorrenteDigito;
                $scope.item.Multa = data.Multa;
                $scope.item.JurosMora = data.JurosDia;
                $scope.item.CodigoTransmissao = data.CodigoTransmissao;

            }).error(function (data) {
                Metronic.unblockUI();

            });
    };



    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Boleto/GerarBoleto", $scope.item)
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    $scope.showBTNOK = false;
                    $scope.showOK = true;
                    $scope.msgOK = "Documento Gerado : " + data.numeroDocumento;
                    $scope.url = data.URL;
                    //$modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;

                    $scope.Erros = data.Erros;
                };
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};



var ControllerEmail = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item;
    $scope.showBTNOK = true;

    $scope.PegaEmail = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Boleto/PegaEmailContato", $scope.item)
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus === "OK") {
                    item.Email = data.Email;
                }
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    };

    $scope.PegaEmail();


    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Boleto/EnviarEmail", $scope.item)
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    $scope.showBTNOK = false;
                    $scope.showOK = true;
                    $scope.msgOK = "Documento Enviado ";
                    //$modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";

                    $scope.Erros = data.Erros;
                };
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};



var ControllerCancelar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item;
    $scope.showBTNOK = true;


    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Boleto/CancelarBoleto", $scope.item)
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    $scope.showBTNOK = false;
                    $scope.showOK = true;
                    $scope.msgOK = "Documento Cancelado ";
                    //$modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";

                    $scope.Erros = data.Erros;
                };
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};




var ControllerGerarArquivoCNAB = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item;
    $scope.showBTNOK = true;

    Metronic.blockUI({
        boxed: true
    });

    $scope.LoadListaBanco = function () {
        $http.post("../../Boleto/ObterBanco")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaBancos = data;
            });
    };
    $scope.LoadListaBanco();

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Boleto/GerarArquivoCNAB", { idbanco: $scope.idbanco })
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    $scope.showBTNOK = false;
                    $scope.showOK = true;
                    $scope.msgOK = "Documento Gerado  ";
                    $scope.url = data.URL;
                    //$modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;

                    $scope.Erros = data.Erros;
                };
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};


var ControllerHistorico = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $http.get("../../Boleto/ObterArquivosHistoricos")
        .success(function (data) {
            $scope.objs = data;
            console.log($scope.objs);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
