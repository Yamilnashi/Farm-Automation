'use strict';
var app = app || {};

app = {
    globalAjaxAlertError: function (enable) {
        $.ajaxSetup({
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (enable) {
                    console.log(XMLHttpRequest, textStatus, errorThrown)
                    if (XMLHttpRequest.responseText.startsWith('<!DOCTYPE')) {
                        alert('oops, something bad happened');
                    } else if (XMLHttpRequest.responseJSON) {
                        alert('Error: ' + `${XMLHttpRequest.responseJSON.error ? XMLHttpRequest.responseJSON.error : XMLHttpRequest.responseJSON}`);
                    } else {
                        const res = JSON.parse(XMLHttpRequest.responseText)
                        alert('Error: ' + res.error);
                    }
                }
            }
        });
    },
    confirm: function (callback, title, message, yesButtonText, noButtonText) {
        var findElement = $('div[data-confirm-modal="true"]')[0];
        if (findElement === undefined) {
            title = title ? title : 'Confirm'

            message = message ? `<div class="modal-body">
                                <div>${message}</div>
                              </div>` : '';

            noButtonText = noButtonText ? noButtonText : 'Close';

            yesButtonText = yesButtonText ? yesButtonText : 'Confirm';
            var $modal = $(`
            <div class="modal fade" tabindex="-1" role="dialog" data-confirm-modal="true" id="cda-confirmation-modal">
              <div class="modal-dialog" role="document">
                <div class="modal-content">
                  <div class="modal-header">
                    <h1 class="modal-title h5">${title}</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                  </div>
                    ${message}
                  <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">${noButtonText}</button>
                    <button type="button" class="btn btn-primary cda-confirmation">${yesButtonText}</button>
                  </div>
                </div>
              </div>
            </div>`).appendTo('body');
            $modal.modal('show')

            $modal.on('hidden.bs.modal', function () {
                $modal.remove();
            }).on('click', 'button.cda-confirmation', function (e) {
                $(this).prop('disabled', true);
                $modal.modal('hide');
                callback(e);
            });
        }
    },
    loadSpinner: function (targartDiv, ajaxUrl, afterLoadCallBack) {
        $(targartDiv).empty();
        $(targartDiv).append(this.getSpinnerDiv())
        $(targartDiv).load(ajaxUrl, afterLoadCallBack);
    },
    loadSpinnerAjax: function (targartDiv, ajaxUrl, data, afterLoadCallBack) {
        $(targartDiv).empty();
        $(targartDiv).append(this.getSpinnerDiv())
        $.ajax({
            url: ajaxUrl, // Replace this with your API endpoint URL
            type: 'GET',
            data: data,
            dataType: 'html', // Expected data type of the response
            success: function (res) {
                // Handle the successful response here
                $(targartDiv).html(res);
                if (afterLoadCallBack) {
                    afterLoadCallBack();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.error('Error:', textStatus, errorThrown);
            }
        });
    },
    getSpinnerDiv: function () {
        return `<div class="my-spinner-div">
          <div class="spinner-grow text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>`
    }
}