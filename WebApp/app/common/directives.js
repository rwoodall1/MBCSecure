'use strict';

angular.module('app')

.directive('signaturepad', function () {
    return {
        restrict: 'E',
        templateUrl: '/v-28/app/common/directiveTemplates/signaturePad_template.html',
        link: function (scope, element, attrs) {
            initialize();

            function initialize() {
                $('.signaturePad').signaturePad(getSignaturePadOptions());
            }

            function getSignaturePadOptions() {
                return {
                    drawOnly: true,
                    canvas: 'canvas.pad',
                    errorMessageDraw: "Please check all the statements above and sign the form.",
                    lineTop: 110,
                    output: '.sigOutput',
                    clear: '.signature-clear-button',
                    onBeforeValidate: function () { },
                    onFormError: function (errors, context, settings) {
                    }
                };
            }
        }
    };
})

.directive('getimagebase64', function() {
    return {
        restrict: 'A',
        scope: {
            getimagebase64: '='
        },
        link: function (scope, elm, attrs) {
            elm.bind('change', function( evt ) {
                scope.$apply(function() {
                    var files=evt.target.files;
                    var file = files[0];
                    if (files && file) {
                        if ((/\.(png|jpeg|jpg|gif)$/i).test(file.name)) {
                            window.URL = window.URL || window.webkitURL;
                            var useBlob = false && window.URL;
                            var reader = new FileReader();
                            reader.addEventListener("load", function () {
                                var image = new Image();
                                image.addEventListener("load", function () {
                                    var imageInfo = file.name + ' ' + // get the value of `name` from the `file` Obj
                                                    image.width + '×' + // But get the width from our `image`
                                                    image.height + ' ' +
                                                    file.type + ' ' +
                                                    Math.round(file.size / 1024) + 'KB';
                                    var canvas = document.createElement('CANVAS');
                                    var ctx = canvas.getContext('2d');
                                    var dataURL;
                                    //Resize image to fit in 640x640 box and set finished product into scope
                                    //TODO: Wire the dimensions up to the MaxImageSize value in ExternalStorageCredentials table.  Figure out if webcam doing the same....
                                    var maxHeight = 640;
                                    var maxWidth = 640;
                                    if (image.height > maxHeight) {
                                        canvas.width = image.width / (image.height / maxHeight);
                                        canvas.height = maxHeight;
                                    } else {
                                        canvas.width = image.width;
                                        canvas.height = image.height;
                                    }
                                    if (canvas.width > maxWidth) {
                                        canvas.height = canvas.height / (canvas.width / maxWidth);
                                        canvas.width = maxWidth;
                                    }
                                    //alert('height:' + canvas.height + ' | width:' + canvas.width);
                                    ctx.drawImage(this, 0, 0, canvas.width, canvas.height);
                                    dataURL = canvas.toDataURL('image/jpeg');
                                    scope.getimagebase64 = dataURL.replace(/^data:image\/(png|jpg|jpeg);base64,/, "");
                                    canvas = null;
                                });

                                image.src = useBlob ? window.URL.createObjectURL(file) : reader.result;
                                if (useBlob) {
                                    window.URL.revokeObjectURL(file);
                                }
                            });
                            reader.readAsDataURL(file);
                        } else {
                           alert("Image format not supported.")
                        }
                    }
                });
            });
        }
    };
})

.directive('formatPhone', [
    function () {
        return {
            require: 'ngModel',
            restrict: 'A',
            link: function (scope, elem, attrs, ctrl, ngModel) {
                elem.add(phonenumber).on('keyup', function () {
                    var origVal = elem.val().replace(/[^\w\s]/gi, '');
                    jQuery("#phonenumber").val(origVal);
                    //if(origVal.length === 10) {
                    //    var str = origVal.replace(/(.{3})/g,"$1-");
                    //    var phone = str.slice(0, -2) + str.slice(-1);
                    //    jQuery("#phonenumber").val(phone);
                    //}
                });
            }
        };
    }
])

.directive('jquerydatepicker', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            var modelName = attrs['datePicker'];
            element.datepicker({
                dateFormat: 'mm/dd/yy',
                onSelect: function (date) {
                    var model = $parse(modelName);
                    model.assign(scope, date);
                    scope.$apply();
                }
            });
        }
    };
}])

// Code to implement modal

//<button data-toggle="modal" data-target="#modal1" class="btn btn-default">Open modal 1</button>
//<button data-toggle="modal" data-target="#modal2" class="btn btn-default">Open modal 2</button>

//<modal title="Modal 1" id="modal1">
//    Modal 1
//</modal>
//<modal title="Modal 2" id="modal2">
//    Modal 2
//</modal>

.directive('modal', function () {
    return {
        template: '<div class="modal fade">' +
            '<div class="modal-dialog">' +
                '<div class="modal-content">' +
                '<div class="modal-header">' +
                    '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                    '<h4 class="modal-title">{{ title }}</h4>' +
                '</div>' +
                '<div class="modal-body" ng-transclude></div>' +
                '</div>' +
            '</div>' +
            '</div>',
        restrict: 'E',
        transclude: true,
        replace: true,
        scope: true,
        link: function postLink(scope, element, attrs) {
            
            scope.title = attrs.title;
            scope.showModal = attrs.visable;

            //Hide or show the modal
            scope.showModal = function (visable) {
                if (visable) {
                    element.modal("show");
                }
                else {
                    element.modal("hide");
                }
            }

           

            //Check to see if the modal-visible attribute exists
            if (attrs.modalVisible == "true") {
                scope.showModal(true);
            }
            else {
                //Watch for changes to the modal-visible attribute
                scope.$watch("modalVisible", function (newValue, oldValue) {
                    scope.showModal(newValue);
                });

                //Update the visible value when the dialog is closed through UI actions (Ok, cancel, etc.)
                element.bind("hide.bs.modal", function () {
                    scope.modalVisible = false;
                    if (!scope.$$phase && !scope.$root.$$phase) {
                        scope.$apply();
                    }
                });
            }
        }
    };
})

//.directive('multiselect', ['$timeout', '$parse', function ($timeout, $parse) {
//    var linker = function ($scope, $element, $attributes, ngModel) {

//        // options for multiselect
//        var name = $attributes.name || '';
//        var options = {};
//        if ($attributes.msHeader)
//            options.header = $attributes.msHeader;
//        if ($attributes.msHeight)
//            options.height = $attributes.msHeight;
//        if ($attributes.msMinWidth)
//            options.minWidth = $attributes.msMinWidth;
//        if ($attributes.msCheckAllText)
//            options.checkAllText = $attributes.msCheckAllText;
//        if ($attributes.msUncheckAllText)
//            options.uncheckAllText = $attributes.msUncheckAllText;
//        if ($attributes.msNoneSelectedText)
//            options.noneSelectedText = $attributes.msNoneSelectedText;
//        if ($attributes.msSelectedText)
//            options.selectedText = $attributes.msSelectedText;
//        if ($attributes.msSelectedList)
//            options.selectedList = $attributes.msSelectedList;
//        if ($attributes.msShow)
//            options.show = $attributes.msShow && $parse($attributes.msShow)($scope) || '';
//        if ($attributes.msHide)
//            options.hide = $attributes.msHide && $parse($attributes.msHide)($scope) || "";
//        if ($attributes.msAutoOpen)
//            options.autoOpen = $attributes.msAutoOpen && $parse($attributes.msAutoOpen)() || false;
//        if ($attributes.msMultiple)
//            options.multiple = $attributes.msMultiple && $parse($attributes.msMultiple)();

//        // events for multiselect
//        if ($attributes.msOpen)
//            options.open = $attributes.msOpen && $parse($attributes.msOpen)($scope);
//        if ($attributes.msBeforeOpen)
//            options.beforeopen = $attributes.msBeforeOpen && $parse($attributes.msBeforeOpen)($scope);
//        if ($attributes.msBeforeClose)
//            options.beforeclose = $attributes.msBeforeClose && $parse($attributes.msBeforeClose)($scope);
//        if ($attributes.msClose)
//            options.close = $attributes.msClose && $parse($attributes.msClose)($scope);
//        if ($attributes.msCheckAll)
//            options.checkAll = $attributes.msCheckAll && $parse($attributes.msCheckAll)($scope);
//        if ($attributes.msUncheckAll)
//            options.uncheckAll = $attributes.msUncheckAll && $parse($attributes.msUncheckAll)($scope);
//        if ($attributes.msClick)
//            options.click = $attributes.msClick && $parse($attributes.msClick)($scope);

//        $scope.$watch($attributes.ngModel, function (modelValue) {
//            // wait till next cycle to load refreshed data onto DOM
//            $timeout(function () {
//                $element.multiselect('refresh');
//            }, 0, false)
//        });

//        if ($attributes.name) {
//            $scope.$on(name, function (event, args) {
//                var method = args.method;
//                if (method) {
//                    $element.multiselect(args.method);
//                }
//                else {
//                    console.error('Method "' + method + '" "does not exist')
//                }
//            });
//        }
//        console.log($attributes);
//        console.log(options);
//        $element.multiselect(options).multiselectfilter();
//        $element.multiselect("checkAll");

//    };
//    return {
//        restrict: 'EA',
//        require: '?ngModel',
//        link: linker
//    }
//}])
//Makes sure document element has been loaded.
.directive('ngElementReady', [function () {
    return {
        priority: Number.MIN_SAFE_INTEGER, // execute last, after all other directives if any.
        restrict: "A",
        link: function ($scope, $element, $attributes) {
            $scope.$eval($attributes.ngElementReady); // execute the expression in the attribute.
        }
    };
}])

.directive('disablecopypaste', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.on('copy paste drag drop', function (e) { e.preventDefault(); });
        }
    }
})
.directive('webcamproof', function () {
    return {
        restrict: 'E',
        templateUrl: '/v-28/app/common/directiveTemplates/webcamProof_template.html',
        scope: {
            image64: '='
        },
        link: function (scope, element, attrs) {
            initialize();

            function initialize() {
                $("#webcamProof").scriptcam({
                    showDebug: false,
                    showMicrophoneErrors: false,
                    onError: onError,
                    disableHardwareAcceleration: 0,
                    cornerColor: 'e3e5e2',
                    onWebcamReady: onWebcamReady,
                    noFlashFound: '<div class="alert alert-danger"><span class="glyphicon glyphicon-warning-sign"></span><strong>You must have flash to use your webcam. Please use the mobile app instead.</strong></div>'
                });
                scope.showspinner = true;
                scope.proofCaptured = false;
                scope.base64ToImage = base64ToImage;
            }

            function base64ToImage() {
                scope.image64 = $.scriptcam.getFrameAsBase64();
                scope.proofCaptured = true;
            }

            function onWebcamReady(cameraNames, camera, microphoneNames, microphone, volume) {
                $("#shutterButton").show();
                // 0 = Camera 
                // 1 = No Camera
                // 2 = ??
                if (cameraNames.length === 0) {
                    $("#webcamProof").hide();
                    $("#shutterButton").hide();
                }
                $("#spinner").hide();
            }

            function onError(errorId, errorMsg) {
                $("#btn1").attr("disabled", true);
                $("#btn2").attr("disabled", true);
                $('#webcamProof').hide();
                alert(errorMsg);
            }
        }
    };
})

.directive('parseStyle', function ($interpolate) {
    return function (scope, elem) {
        var exp = $interpolate(elem.html()),watchFunc = function () { return exp(scope); };

        scope.$watch(watchFunc, function (html) {
            elem.html(html);
        });
    };
})

//    Validation 
.directive('zipcodevalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var CCREGEX5 = new RegExp("^\\d{5}(?:[-\\s]\\d{4})?$");

            var checkErrors = function (modelValue, viewValue) {
                if (CCREGEX5.test(viewValue)) {
                    return true;
                }
                return false;
            };
            ctrl.$validators.invalid = checkErrors;
        }
    };
})


.directive('telephonenumbervalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var CCREGEX = new RegExp("^\\d{3}[-\\s]?\\d{3}[-\\s]?\\d{4}$");
            var checkErrors = function (modelValue, viewValue) {
                if (CCREGEX.test(viewValue)) {
                    return true;
                }
                return false;
            };
            ctrl.$validators.invalid = checkErrors;
        }
    };
}
)
.directive('validdollaramount', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {

            scope.$watch('commission.amount', function (newValue, oldValue) {
                if (newValue) {
                    var CCREGEX = new RegExp(/^[0-9]*\.?[0-9]?[0-9]?$/);

                    var checkErrors = function (modelValue) {
                        if (CCREGEX.test(modelValue)) {
                            return true;
                        }
                        return false;
                    };

                    ctrl.$validators.invalid = checkErrors;
                }
            }, true);
        }
    };
})

.directive('emailvalidate', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {

            var EMAIL_REGEXP = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;

            ctrl.$validators.email = function (modelValue, viewValue) {
                if (ctrl.$isEmpty(modelValue)) {
                    // consider empty models to be valid
                    return true;
                }

                if (EMAIL_REGEXP.test(viewValue)) {
                    // it is valid
                    return true;
                }

                // it is invalid
                return false;
            };
        }
    };
})
.directive('maxfivenumbers', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var CCREGEX = new RegExp("^[0-9]{5}$");

            var checkErrors = function (modelValue, viewValue) {
                if (CCREGEX.test(viewValue)) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('last4ssnvalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var CCREGEX = new RegExp("^[0-9]{4}$");

            var checkErrors = function (modelValue, viewValue) {
                if (CCREGEX.test(viewValue)) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('fullssnvalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var CCREGEX = new RegExp("^[0-9]{9}$");

            var checkErrors = function (modelValue, viewValue) {
                if (CCREGEX.test(viewValue)) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('emailvalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var CCREGEX = new RegExp("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$");

            var checkErrors = function (modelValue, viewValue) {
                if (viewValue == "" || viewValue == null) {
                    return true;
                }

                if (CCREGEX.test(viewValue)) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('dobvalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var CCREGEX = new RegExp("^[0-9][0-9]\/[0-9][0-9]\/[0-9][0-9][0-9][0-9]$");

            var checkErrors = function (modelValue, viewValue) {
                if (CCREGEX.test(viewValue)) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('datedayvalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var checkErrors = function (modelValue, viewValue) {
                var value = parseInt(viewValue);

                if (value >= 1 && value <= 31) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('datemonthvalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var checkErrors = function (modelValue, viewValue) {
                var value = parseInt(viewValue);

                if (value >= 1 && value <= 12) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('dateyearvalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var checkErrors = function (modelValue, viewValue) {
                var value = parseInt(viewValue);
                var currentYear = new Date().getFullYear();

                if (value >= 1900 && value <= currentYear) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('nameinitialsvalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var checkErrors = function (modelValue, viewValue) {
                if (viewValue === undefined) {
                    return false;
                }

                if (viewValue.length == 2 || viewValue.length == 3) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.invalid = checkErrors;
        }
    };
})
.directive('checklistModel', ['$parse', '$compile', function ($parse, $compile) {
    // contains
    function contains(arr, item, comparator) {
        if (angular.isArray(arr)) {
            for (var i = arr.length; i--;) {
                if (comparator(arr[i], item)) {
                    return true;
                }
            }
        }
        return false;
    }

    // add
    function add(arr, item, comparator) {
        arr = angular.isArray(arr) ? arr : [];
        if (!contains(arr, item, comparator)) {
            arr.push(item);
        }
        return arr;
    }

    // remove
    function remove(arr, item, comparator) {
        if (angular.isArray(arr)) {
            for (var i = arr.length; i--;) {
                if (comparator(arr[i], item)) {
                    arr.splice(i, 1);
                    break;
                }
            }
        }
        return arr;
    }

    // http://stackoverflow.com/a/19228302/1458162
    function postLinkFn(scope, elem, attrs) {
        // exclude recursion, but still keep the model
        var checklistModel = attrs.checklistModel;
        attrs.$set("checklistModel", null);
        // compile with `ng-model` pointing to `checked`
        $compile(elem)(scope);
        attrs.$set("checklistModel", checklistModel);

        // getter / setter for original model
        var getter = $parse(checklistModel);
        var setter = getter.assign;
        var checklistChange = $parse(attrs.checklistChange);

        // value added to list
        var value = attrs.checklistValue ? $parse(attrs.checklistValue)(scope.$parent) : attrs.value;


        var comparator = angular.equals;

        if (attrs.hasOwnProperty('checklistComparator')) {
            if (attrs.checklistComparator[0] == '.') {
                var comparatorExpression = attrs.checklistComparator.substring(1);
                comparator = function (a, b) {
                    return a[comparatorExpression] === b[comparatorExpression];
                }

            } else {
                comparator = $parse(attrs.checklistComparator)(scope.$parent);
            }
        }

        // watch UI checked change
        scope.$watch(attrs.ngModel, function (newValue, oldValue) {
            if (newValue === oldValue) {
                return;
            }
            var current = getter(scope.$parent);
            if (angular.isFunction(setter)) {
                if (newValue === true) {
                    setter(scope.$parent, add(current, value, comparator));
                } else {
                    setter(scope.$parent, remove(current, value, comparator));
                }
            }

            if (checklistChange) {
                checklistChange(scope);
            }
        });

        // declare one function to be used for both $watch functions
        function setChecked(newArr, oldArr) {
            scope[attrs.ngModel] = contains(newArr, value, comparator);
        }

        // watch original model change
        // use the faster $watchCollection method if it's available
        if (angular.isFunction(scope.$parent.$watchCollection)) {
            scope.$parent.$watchCollection(checklistModel, setChecked);
        } else {
            scope.$parent.$watch(checklistModel, setChecked, true);
        }
    }

    return {
        restrict: 'A',
        priority: 1000,
        terminal: true,
        scope: true,
        compile: function (tElement, tAttrs) {
            if ((tElement[0].tagName !== 'INPUT' || tAttrs.type !== 'checkbox')
                && (tElement[0].tagName !== 'MD-CHECKBOX')
                && (!tAttrs.btnCheckbox)) {
                throw 'checklist-model should be applied to `input[type="checkbox"]` or `md-checkbox`.';
            }

            if (!tAttrs.checklistValue && !tAttrs.value) {
                throw 'You should provide `value` or `checklist-value`.';
            }

            // by default ngModel is 'checked', so we set it if not specified
            if (!tAttrs.ngModel) {
                // local scope var storing individual checkbox model
                tAttrs.$set("ngModel", "checked");
            }

            return postLinkFn;
        }
    };
}])
.directive('positivenumbervalidation', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            var checkErrors = function (modelValue, viewValue) {
                if (viewValue === undefined || viewValue === "") {
                    return true;
                }

                var value = parseInt(viewValue);

                if (value >= 0) {
                    return true;
                }
                return false;
            };

            ctrl.$validators.nonpositivenumber = checkErrors;
        }
    };
});

