(function($) {

	var pluginObject = {

		options: {
			jobId: null,
			orderId: null,
			projectId: null
		},
		elem: null,
		$elem: null,
		$postLinks: null,
		$downloadPackageLink: null,
		$copyWordCountToJobsAction: null,
		$copyWordCountToOrdersAction: null,
		$copyJobWordCountToAllJobsAndRecalculate: null,
		$copyJobWordCountToAllJobsAndRecalculateAndJumpstartDeadlines: null,
		$copyJobWordCountToAllOrdersAndJobsAndRecalculateAndUpdateJumpstartDeadline: null,
		$copyJobWordCountToAllOrdersAndJobsAndRecalculate: null,
		$acceptConfidentialAgreement: null,
		$acceptCreateBidLinks: null,
		$acceptBidButton: null,
		$createBidButton: null,
		$orderJobsTable: null,
		$projectOrdersTable: null,
		$orderWordCountField: null,
		$wordCountField: null,
		isDynamicMTPricingUsedUrl: null,
		wordCountStatisticsFields: {
			customer: {
				$wordCount: null,
				$repetitions: null,
				$percent100: null,
				$percent95to99: null,
				$percent85to94: null,
				$percent75to84: null,
				$percent50to74: null,
				$noMatch: null,
				$contextMatch: null
			},
			vendor: {
				$wordCount: null, 
				$repetitions: null,
				$percent100: null,
				$percent95to99: null,
				$percent85to94: null,
				$percent75to84: null,
				$percent50to74: null,
				$noMatch: null,
				$contextMatch: null
			}
		},
		$oasForOnlineButton: null,
		$parentOfOasForOnlineButton: null,
		$jumpstartPriceField: null,
		$customerPriceField: null,
		$discountField: null,
		$translationMemoryDiscountField: null,
		$markUp: null,
		$form: null,
		recalculateMarkUpUrl: null,
		$finishJobAction: null,
		$sourceFilesTable: null,
		$sourceLanguageField: null,
		$targetLanguagesField: null,
		$referenceFilesTable: null,
		$extractTargetFromSdlxliffButton: null,
		getOnlineFlowMessageUrl: null,
		$workUnitField: null,
		$numberOfUnitsField: null,
		$wordCountFieldContextMenu: null,
		$numberOfUnitsFieldContextMenu: null,
		$wWCPricePerUnitField: null,
		$weightedWordCount: null,
		copyWordCountStatisticUrl: null,
		$supplierworkUnitField: null,
		$supplierNumberOfUnitsField: null,
		$supplierNumberOfUnitsFieldContextMenu: null,
		onlineWorkflowFilesNotificationUrl: null,
		$isDynamicPricingUsedField: null,
		$isDynamicMTPricingUsedField: null,
		$isDynamicPricingUsedEnabled: null,

		_initProperties: function() {
			var self = this;

			self.$postLinks = self.$elem.find('a.post');
			self.$downloadPackageLink = self.$elem.find('.downloadArchive');
			
			self._loadWordcountFields();

			self.$acceptConfidentialAgreement = self.$elem.find('.acceptConfidentialAgreement');
			self.$acceptCreateBidLinks = self.$elem.closest('div.submitPage').find('.acceptBid, .createBid');
			self.$acceptCreateBidButtons = self.$elem.closest('div.submitPage').find('.acceptBid .button, .createBid .button');
			self.$orderJobsTable = $('#orderJobsTable_' + self.options.orderId);
			self.$projectOrdersTable = $('#projectOrders_' + self.options.projectId);
			self.$orderWordCountField = $('.orderWordCountField');
			self.$jumpstartPriceField = self.$elem.find('.jumpstartPrice');
			self.$customerPriceField = self.$elem.find('.customerPrice');
			self.$wWCPricePerUnitField = self.$elem.find('.wWCPricePerUnit');
			self.$weightedWordCount = self.$elem.find('.weightedWordCount');
			self.$discountField = self.$elem.find('.discount');
			self.$translationMemoryDiscountField = self.$elem.find('.translationMemoryDiscount');
			self.$markUp = self.$elem.find('.markUp');
			self.$form = self.$elem.findAndFilter('.form');
			self.$oasForOnlineButton = self.$elem.find('.oasForOnline');
			self.$parentOfOasForOnlineButton = self.$oasForOnlineButton.closest('.calculateOasForOnlineProject');
			self.recalculateMarkUpUrl = self.$elem.find('#recalculateMarkUpUrl').val();
			self.$finishJobAction = self.$elem.find('.finishJobAction');
			self.$sourceFilesTable = $('#jobSourceFilesTable_' + self.options.jobId);
			self.$referenceFilesTable = $('#jobProjectReferenceFilesTable_' + self.options.jobId);
			self.$sourceLanguageField = self.$elem.find('.sourceLanguage');
			self.$targetLanguagesField = self.$elem.find('.targetLanguages');
			self.$extractTargetFromSdlxliffButton = $('.extractTargetFromSdlxliff');
			self.getOnlineFlowMessageUrl = self.$elem.find('#getOnlineFlowMessageUrl').val();
			self.$jumpstartDealineField = self.$elem.find('.field.jumpstartDeadline');
			self.$matchedResourcesGrid = $(`#jobMatchedSuppliers${self.options.jobId}`);
			self.$workUnitField = self.$elem.find('.workUnit');
			self.$numberOfUnitsField = self.$elem.find('.numberOfUnits');
			self.copyWordCountStatisticUrl = self.$elem.find('#copyWordCountStatisticUrl').val();
			self.$supplierworkUnitField = self.$elem.find('.supplierWorkUnit');
			self.$supplierNumberOfUnitsField = self.$elem.find('.supplierNumberOfUnits');
			self.onlineWorkflowFilesNotificationUrl = self.$elem.find('#OnlineWorkflowFilesNotificationUrl').val();
			self.$isDynamicMTPricingUsedField = self.$elem.find('.isDynamicMTPricingUsed.field');
			self.isDynamicMTPricingUsedUrl = self.$elem.find('#isDynamicMTPricingUsedUrl').val();
			self.$isDynamicPricingUsedEnabled = self.$elem.find('#IsDynamicPricingUsedEnabled');
			self.$isDynamicPricingUsedField = self.$elem.find('.isDynamicPricingUsed.field');
		},

		_loadWordcountFields: function () {
			var self = this;
			var wordCountTableId = self.$elem.find(".word-count-results-section-component table[id*='-']").prop("id");

			self.$wordCountTooltip = self.$elem.find('.word-count-results-section-component [rel=tooltip]')
			self.$wordCountField = self.$elem.find('.wordCount');
			self.wordCountStatisticsFields.customer.$wordCount = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_WordCount]');
			self.wordCountStatisticsFields.customer.$repetitions = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_Repetitions]');
			self.wordCountStatisticsFields.customer.$noMatch = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_NoMatch]');
			self.wordCountStatisticsFields.customer.$contextMatch = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_ContextMatch]');
			self.wordCountStatisticsFields.customer.$percent100 = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_Percent100]');
			self.wordCountStatisticsFields.customer.$percent95to99 = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_Percent95To99]');
			self.wordCountStatisticsFields.customer.$percent85to94 = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_Percent85To94]');
			self.wordCountStatisticsFields.customer.$percent75to84 = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_Percent75To84]');
			self.wordCountStatisticsFields.customer.$percent50to74 = self.$elem.find('tr[id=1] [aria-describedby=' + wordCountTableId + '_Percent50To74]');


			self.wordCountStatisticsFields.vendor.$wordCount = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_WordCount]');
			self.wordCountStatisticsFields.vendor.$repetitions = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_Repetitions]');
			self.wordCountStatisticsFields.vendor.$noMatch = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_NoMatch]');
			self.wordCountStatisticsFields.vendor.$contextMatch = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_ContextMatch]');
			self.wordCountStatisticsFields.vendor.$percent100 = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_Percent100]');
			self.wordCountStatisticsFields.vendor.$percent95to99 = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_Percent95To99]');
			self.wordCountStatisticsFields.vendor.$percent85to94 = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_Percent85To94]');
			self.wordCountStatisticsFields.vendor.$percent75to84 = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_Percent75To84]');
			self.wordCountStatisticsFields.vendor.$percent50to74 = self.$elem.find('tr[id=2] [aria-describedby=' + wordCountTableId + '_Percent50To74]');
		},

		_initTmDropdown: function () {
			var self = this;
			var $toolbar = self.$elem.find('.tmx-toolbar');
			var $link = self.$elem.find('.tmx-toolbar + ul .download-tmx-action');
			$link.bind('click.tmxDropdown', function (e) {
				e.preventDefault();
				return false;
			});
			var checkUrl = $link.data('additional');
			$toolbar.on('click', function () {
				setTimeout(function () {
					$.lw_ajax({
						url: checkUrl,
						type: 'GET',
						success: function (data) {
							if (data && data.isNotify) {
								showNotificationFromData(data)
							}
							else {
								if (data.Url) {
									$link.prop('href', data.Url);
									$link.prop('disabled', '');
									$link.unbind('click.tmxDropdown');
									$link.removeClass('disabled');
								}
								else {
									$link.prop('title', data.Message);
								}
							}
						},
						$loadingIndicatorArea: $link
					});
				});
			});
			
		},

		_initCopyWordCountAction: function () {
			var self = this;
			var wordCountTableId = self.$elem.find(".word-count-results-section-component table[id*='-']").prop("id");

			var observer = new MutationObserver(function (mutations) {
				let copyActionColumn = self.$elem.find('#jqgh_' + wordCountTableId + '_CopyStatisticAction');
				if (copyActionColumn.length) {
					copyActionColumn.html('');
					observer.disconnect();
				}
			});
			observer.observe(document, { attributes: false, childList: true, characterData: false, subtree: true });

			self.$elem.on('click', '.copy-customer-statistic', function (e) {
				var data = {
					id: self.options.jobId,
					isCopyCustomerWordCount: true
				};
				$.lw_ajax({
					url: self.copyWordCountStatisticUrl,
					data: data,
					success: function (data) {
						self._refreshPage();
					},
					$loadingIndicatorArea: self.$elem
				});
			});

			self.$elem.on('click', '.copy-supplier-statistic', function (e) {
				var data = {
					id: self.options.jobId,
					isCopyCustomerWordCount: false
				};
				$.lw_ajax({
					url: self.copyWordCountStatisticUrl,
					data: data,
					success: function (data) {
						self._refreshPage();
					},
					$loadingIndicatorArea: self.$elem
				});
			});
		},

		init: function(options, elem) {
			var self = this;
			self.options = $.extend({}, self.options, options);

			self.elem = elem;
			self.$elem = $(elem);
			self._initProperties();
			self._initDownloadPackagelink();
			self._initConfidantialityRelatedButtons();
			self._initAcceptConfidentialAgreement();
			self._initDisableClick();
			self._initSuggestPriceMenu();
			self._initBidExpandableRowReloadOnSubmit();
			self._initSecondaryActionsReload();
			self._initTooltips();
			self._initMarkUpUpdateLogic();
			self._initOrderPriceFieldsUpdateLogic();
			self._initFinishJobAction();
			self._initOasForOnlineHandler();
			self._initUpdatePageAfterChangeFieldsHandler();
			self._initExtractTargetFromSdlxliffHandler();
			self._initJumpstartDeadline();
			self._initWorkUnitChangedHandler();
			self._initUpdatePriceLogic();
			self._initCopyWordCountHandler();
			self._initGlobalSupplierPriceDiscount();
			self._initwWCPricePerUnitFiels();
			self._initCopyWordCountAction();
			self._updateAllowedUseCharactersWarning();
			self._showOnlineWorkflowFilesNotification();
			self._initIsDynamicMTPricingUsedField();
			self._initTmDropdown();

			if (window.dataLayer) {
				if (self.options.projectId)
					dataLayer.push({ "projectId": self.options.projectId });

				dataLayer.push({ "jobId": self.options.jobId });
				dataLayer.push({ "assignmentId": self.options.orderId });
				dataLayer.push({ "entityId": self.$form.find('input#CompanyId').val() });
			}

			return this;
		},

		_showOnlineWorkflowFilesNotification: function () {
			let self = this;
			self.$elem.sendRequest('sendRequest', {
				url: self.onlineWorkflowFilesNotificationUrl
			});
		},

		_updateAllowedUseCharactersWarning: function () {
			let self = this;
			let warningMassage = self.$elem.closest('.submitPage').find('.entity-not-configurate-use-characters');

			if (!warningMassage.length) return;

			warningMassage.toggleClass('hidden', self.$workUnitField.find('select').val() != 6);
		},

		_initwWCPricePerUnitFiels: function () {
			let self = this;
			if (!self.$wWCPricePerUnitField.length) {
				return;
			}

			let wWCPricePerUnitFieldInput = self.$wWCPricePerUnitField.find('input');
			if (wWCPricePerUnitFieldInput.length) {
				wWCPricePerUnitFieldInput[0].setAttribute('readonly', true)
			}

			self.$form.on('successfulFormSubmit', (e, data) => {
				self._updateWWCPricePerUnitField(data);
			});
		},

		_initIsDynamicMTPricingUsedField: function () {
			let self = this;
			if (!self.$isDynamicMTPricingUsedField.length) return;

			if (self.$isDynamicPricingUsedEnabled.val() == 'True') {
				self.$isDynamicMTPricingUsedField.removeClass('hidden');
				self.$isDynamicPricingUsedField.removeClass('hidden');
			} else {
				self.$isDynamicMTPricingUsedField.addClass('hidden');
				self.$isDynamicPricingUsedField.addClass('hidden');
			}

			self.$form.on('successfulFormSubmit', (e, data) => {
				setTimeout(function () {
					self._updateIsDynamicMTPricingUsedField();
				}, 1000)
			});
		},

		_initGlobalSupplierPriceDiscount: function () {
			var self = this;
			var $globalSupplierPriceDiscountFieldsGroup =
				self.$form.find('.globalSupplierPriceDiscountFieldsGroup');
			if ($globalSupplierPriceDiscountFieldsGroup != undefined) {
				setTimeout(function () {
					scrollToElement($globalSupplierPriceDiscountFieldsGroup);
				}, 500)
			}
		},

		_initWorkUnitChangedHandler: function() {
			const self = this;
			self.$form.on('successfulFormSubmit', (e, data) => {
				self._updateNumberOfUnitsField(data);
				self._updateAllowedUseCharactersWarning();
			});

			if (self.$workUnitField.length) {
				self.$workUnitField.numberOfUnits();
			}

			self._initNumberOfUnitsTooltip();
			self._initSupplierNumberOfUnitsTooltip();
		},

		_initNumberOfUnitsTooltip: function () {
			const self = this;
			if (!self.$workUnitField.length) {
				return;
			}

			var isVisible = self.$form.find('#IsCustomerNumberOfUnitsVisible').val();
			let tooltip = self.$numberOfUnitsField.find('.jumpstart-question-mark');
			if (tooltip.length && isVisible == 'False') {
				tooltip.attr('style', 'display: none');
			}
			else {
				tooltip.attr('style', '');
			}
		},

		_initSupplierNumberOfUnitsTooltip: function () {
			const self = this;
			if (!self.$supplierworkUnitField.length) {
				return;
			}
			var isVisible = self.$form.find('#IsSupplierNumberOfUnitsVisible').val();
			let tooltip = self.$supplierNumberOfUnitsField.find('.jumpstart-question-mark');
			if (tooltip.length) {
				if (isVisible == 'False') {
					tooltip.attr('style', 'display: none');
				} else {
					tooltip.attr('style', '');
				}
			}
			let contextMenu = self.$supplierNumberOfUnitsField.find('.contextMenu');
			if (contextMenu.length) {
				let selectedWorUnitValue = self.$supplierworkUnitField.find('select').val();
				if (selectedWorUnitValue == 1 || selectedWorUnitValue == 5 || selectedWorUnitValue == 6 || selectedWorUnitValue == 7) {
					contextMenu.attr('style', 'display: none');
				}
				else {
					contextMenu.attr('style', '');
				}
			}
		},

		_updateNumberOfUnitsField: function (data) {
			const self = this;
			let numberOfUnits;
			let $numberOfUnitsField = self.$form.find('.numberOfUnits input[type="number"]');
			if (data && typeof data.NumberOfUnits != 'undefined') {
				numberOfUnits = data.NumberOfUnits;
			}
			else if (data && data.RowData && typeof data.RowData.NumberOfUnits != 'undefined') {
				numberOfUnits = data.RowData.NumberOfUnits;
			}

			if (typeof numberOfUnits != 'undefined') {
				$numberOfUnitsField.val(numberOfUnits);
			}

			let supplierNumberOfUnits;
			let $supplierNumberOfUnitsField = self.$form.find('.supplierNumberOfUnits input[type="number"]');
			if (data && typeof data.SupplierNumberOfUnits != 'undefined') {
				supplierNumberOfUnits = data.SupplierNumberOfUnits;
			}
			else if (data && data.RowData && typeof data.RowData.SupplierNumberOfUnits != 'undefined') {
				supplierNumberOfUnits = data.RowData.SupplierNumberOfUnits;
			}

			if (typeof supplierNumberOfUnits != 'undefined') {
				$supplierNumberOfUnitsField.val(supplierNumberOfUnits);
			}

			let supplierNumberOfUnitsTooltip = self.$supplierNumberOfUnitsField.find('.jumpstart-question-mark');
			if (data && typeof data.IsSupplierNumberOfUnitsVisible != 'undefined') {
				if (data.IsSupplierNumberOfUnitsVisible == true) {
					supplierNumberOfUnitsTooltip.attr('style', '');
				}
				else {
					supplierNumberOfUnitsTooltip.attr('style', 'display: none');
				}
			}
			else if (data && data.RowData && typeof data.RowData.IsSupplierNumberOfUnitsVisible != 'undefined') {
				if (data.RowData.IsSupplierNumberOfUnitsVisible == 'True') {
					supplierNumberOfUnitsTooltip.attr('style', '');
				}
				else {
					supplierNumberOfUnitsTooltip.attr('style', 'display: none');
				}
			}

			let numberOfUnitsTooltip = self.$numberOfUnitsField.find('.jumpstart-question-mark');
			if (data && typeof data.IsCustomerNumberOfUnitsVisible != 'undefined') {
				if (data.IsCustomerNumberOfUnitsVisible == true) {
					numberOfUnitsTooltip.attr('style', '');
				}
				else {
					numberOfUnitsTooltip.attr('style', 'display: none');
				}
			}
			else if (data && data.RowData && typeof data.RowData.IsCustomerNumberOfUnitsVisible != 'undefined') {
				if (data.RowData.IsCustomerNumberOfUnitsVisible == 'True') {
					numberOfUnitsTooltip.attr('style', '');
				}
				else {
					numberOfUnitsTooltip.attr('style', 'display: none');
				}
			}
			let selectedWorUnitValue = self.$supplierworkUnitField.find('select').val();
			if (selectedWorUnitValue == 1) {
				$('.jobMatchedSuppliers').trigger('reloadGrid');
			}
			let contextMenu = self.$supplierNumberOfUnitsField.find('.contextMenu');
			if (contextMenu.length) {
				if (selectedWorUnitValue == 1 || selectedWorUnitValue == 5 || selectedWorUnitValue == 6 || selectedWorUnitValue == 7) {
					contextMenu.attr('style', 'display: none');
				}
				else {
					contextMenu.attr('style', '');
				}
			}
		},

		_updateWWCPricePerUnitField: function (data) {
			let self = this;
			if (data.WWCPricePerUnit && self.$wWCPricePerUnitField.length) {
				self.$wWCPricePerUnitField.find('input').val(data.WWCPricePerUnit);
				self.$weightedWordCount.find('input').val(data.WeightedWordCount);
			}
		},

		_updateCustomerPriceField: function (data) {
			const self = this;
			if (data.RowData && data.RowData.CustomerPrice) {
				self.$elem.jobPrice('setCustomerPriceToInput', parseFloat(data.RowData.CustomerPrice.split(' ')[0]));
			}
		},

		_updateIsDynamicMTPricingUsedField: function () {
			let self = this;
			if (self.$isDynamicMTPricingUsedField.length == 0
				|| self.$isDynamicMTPricingUsedField.hasClass('readonly')) return;

			$.lw_ajax({
				url: self.isDynamicMTPricingUsedUrl,
				type: 'GET',
				success: function (data) {
					self.$isDynamicMTPricingUsedField.find('select').select('setValWithoutSubmit', data.IsDynamicMTPricingUsed);
					self.$isDynamicPricingUsedEnabled.val(data.IsDynamicPricingEnabled)
					if (data.IsDynamicPricingEnabled == 'True') {
						self.$isDynamicMTPricingUsedField.removeClass('hidden');
						self.$isDynamicPricingUsedField.removeClass('hidden');
					} else {
						self.$isDynamicMTPricingUsedField.addClass('hidden');
						self.$isDynamicPricingUsedField.addClass('hidden');
					}
				},
				$loadingIndicatorArea: self.$elem
			});
			
		},

		_initJumpstartDeadline: function() {
			var self = this;
			self.$jumpstartDealineField.on('fieldValueUpdated', () => {
				self.$matchedResourcesGrid.trigger('reloadGrid');
			});
		},

		_initTooltips: function() {
			var self = this;
			self.$elem.find('[rel="tooltip"]').smallipop();
		},

		_initDownloadPackagelink: function() {
			var self = this;
			self.$downloadPackageLink.click(function() {
				var $link = $(this);
				$.lw_ajax({
					url: $link.attr('href'),
					success: function(data) {
						$(data).downloadFilesArchiveForm();
					},
					$loadingIndicatorArea: self.$elem
				});
				return false;
			});
		},

		_initSecondaryActionsReload: function() {
			var self = this;
			self.$elem.find('#jobDeliveredFilesTable_' + $('#Data_Id').val()).on(
				'successSendRequest',
				function(event, elem) {
					var $action = elem.$elem;
					if ($action.hasClass('addNewFiles')
						|| $action.hasClass('deleteFiles')
						|| $action.hasClass('deleteFile')) {
						self.$form.form('reloadSecondaryActions');
					}
				});
		},

		_initSuggestPriceMenu: function() {
			var self = this;

			var $suggestJumpstartPriceMenu = self.$elem.find('.suggestJumpstartPriceMenu');
			if ($suggestJumpstartPriceMenu.length) {
				$suggestJumpstartPriceMenu.priceSuggestionMenu({
					setPriceWithDiscount: true
				});
			}

			var $suggestCutomerPriceMenu = self.$elem.find('.suggestCutomerPriceMenu');
			if ($suggestCutomerPriceMenu.length) {
				$suggestCutomerPriceMenu.priceSuggestionMenu({
					setPriceWithDiscount: true
				});
			}
		},

		_initConfidantialityRelatedButtons: function() {
			var self = this;
			if (self.$acceptConfidentialAgreement.length > 0) {
				self.$acceptCreateBidButtons.addClass('disabled');
				self.$acceptCreateBidButtons.attr('disabled', 'disabled');

				self.$acceptCreateBidLinks.addClass('disabled');
			}
		},

		_initAcceptConfidentialAgreement: function() {
			var self = this;
			self.$acceptConfidentialAgreement.click(function(e) {
				e.preventDefault();
				self.$acceptCreateBidButtons.removeClass('disabled');

				self.$acceptCreateBidLinks.removeClass('disabled');
			});
		},

		_initDisableClick: function() {
			var self = this;
			self.$acceptCreateBidLinks.on('click', '.disabled, .button.disabled', function(e) {
				e.preventDefault(); // remove redirect from browser
				e.stopPropagation(); // remove redirect from jNavigate
			});
		},

		_initBidExpandableRowReloadOnSubmit: function() {
			var $matchedSuppliersGrid = this.$elem.find('.jobMatchedSuppliers');
			$matchedSuppliersGrid.on('expandableRowContentSubmitted', function(e, data) {
				if (data.oldData.Status != data.responseData.Status) {
					$(window).trigger($.Event('navigate'), { RedirectTo: document.URL });
				} else {
					$matchedSuppliersGrid.jqGrid('collapseSubGridRow', data.rowId);
					$matchedSuppliersGrid.jqGrid('expandSubGridRow', data.rowId);
				}

			});
		},

		_initCopyWordCountHandler: function() {
			var self = this;

			self.$wordCountFieldContextMenu = self.$wordCountField.find('.contextMenu');
			if (self.$wordCountFieldContextMenu.length) {
				self.$wordCountFieldContextMenu.on('click', '.scriptHandledAction', function (e) {
					self._wordCountHandler(e, e.currentTarget.href);
				});
			}

			self.$numberOfUnitsFieldContextMenu = self.$numberOfUnitsField.find('.contextMenu');
			if (self.$numberOfUnitsFieldContextMenu.length) {
				self.$numberOfUnitsFieldContextMenu.on('click', '.scriptHandledAction', function (e) {
					if (e.currentTarget.dataset.isConfirm == undefined) {
						self._wordCountHandler(e, e.currentTarget.href);
					}
				});
			}

			self.$supplierNumberOfUnitsFieldContextMenu = self.$supplierNumberOfUnitsField.find('.contextMenu');
			if (self.$supplierNumberOfUnitsFieldContextMenu.length) {
				self.$supplierNumberOfUnitsFieldContextMenu.on('click', '.scriptHandledAction', function (e) {
					if (e.currentTarget.dataset.isConfirm == undefined) {
						self._wordCountHandler(e, e.currentTarget.href);
						$(window).trigger($.Event('navigate'), { RedirectTo: document.URL });
					}
				});
			}

			var $table = self.$wordCountField.closest('.ui-jqgrid-btable');
			if ($table.length) {
				$table.on(
					'successSendRequest',
					function (event, info) {
						self._handleMenuClickResponseResult(info.data);
					});
			}
		},

		_wordCountHandler: function (e, url) {
			var self = this;
			e.stopPropagation();
			e.preventDefault(); 

			var loadingIndicatorArea = false;
			if (typeof (self.$orderJobsTable[0]) != 'undefined') {
				loadingIndicatorArea = self.$orderJobsTable;
			}
			$.lw_ajax({
				url: url,
				success: function(data) {
					self._handleMenuClickResponseResult(data);
				},
				$loadingIndicatorArea: loadingIndicatorArea
			});
		},

		_handleMenuClickResponseResult: function (data) {
			var self = this;
			if (data && data.isNotify) {
				showNotificationFromData(data);
			}
			var wordCountToCopy = self.$wordCountField.find('input').val();
			if (self.$orderWordCountField.length) {
				self.$orderWordCountField.find('input').not(':hidden').val(wordCountToCopy);
			}
			if (self.$customerPriceField && data.additionalData && data.additionalData.newCustomerPrice != null) {
				self.$customerPriceField.find('input')
					.not(':hidden')
					.val(data.additionalData.newCustomerPrice);
			}
			self._updateNumberOfUnitsField(data);
			self._updateCustomerPriceField(data);
			self._updateWWCPricePerUnitField(data);
			$('body').click();
		},

		_initMarkUpUpdateLogic: function() {
			var self = this;

			if (self.$customerPriceField.length) {
				self.$customerPriceField.updateMarkUpOnFieldValueChange({ orderId: self.options.orderId, preventOrderRowUpdate: true });
			}
			if (self.$discountField.length) {
				self.$discountField.updateMarkUpOnFieldValueChange({ orderId: self.options.orderId });
			}
			if (self.$translationMemoryDiscountField.length) {
				self.$translationMemoryDiscountField.updateMarkUpOnFieldValueChange({ orderId: self.options.orderId });
			}
			if (self.$jumpstartPriceField.length) {
				self.$jumpstartPriceField.updateMarkUpOnFieldValueChange({ orderId: self.options.orderId });
			}

			if (self.$markUp.length) {
				self.$form.on('markUpElementChanged', function(e, args) {
					$.lw_ajax({
						url: self.recalculateMarkUpUrl,
						success: function(data) {
							self.$markUp.find('.uneditable-field-value').html(data).attr('title', data);
							self.$markUp.find('input:hidden').val(data);
						},
						$loadingIndicatorArea: self.$markUp
					});
				});
			}
		},

		_initUpdatePriceLogic: function () {
			var self = this;
			self.$elem.jobPrice();
		},

		_initOrderPriceFieldsUpdateLogic: function() {
			var self = this;
			self.$customerPriceField
				.add(self.$discountField)
				.add(self.$translationMemoryDiscountField)
				.add(self.$jumpstartPriceField)
				.add(self.$wordCountField)
				.on('fieldValueUpdated', function() {
					self.$form.closest('.ui-subgrid').closest('.submitPage').trigger($.Event('jobPriceChanged'));
					$('#jobMatchedSuppliers' + self.options.jobId).trigger('reloadGrid');
				});
		},

		_initFinishJobAction: function() {
			var self = this;
			self.$finishJobAction.on('click', function() {
				// The confirm data is added when we receive the server response. But we want the action to only use
				// the confirm data from the server so we need to manually remove the already added confirm data.
				self.$finishJobAction.removeData('url');
				self.$finishJobAction.removeData('isConfirm');
				self.$finishJobAction.removeData('confirmBodySource');
				self.$finishJobAction.removeData('confirmBody');
				self.$finishJobAction.removeData('confirmTitle');
				self.$finishJobAction.removeData('confirmOkButtonLabel');
				self.$finishJobAction.removeData('confirmCancelButtonLabel');
				self.$finishJobAction.removeData('confirmHasChildConfirm');
				self.$finishJobAction.removeData('confirmSubmitUrl');
				self.$finishJobAction.removeData('link');
			});
		},

		_initOasForOnlineHandler: function() {
			var self = this;
			self.$oasForOnlineButton.on('actionCommand', function(e, args) {
				if (args.command == 'calculateOasForOnlineProject') {
					if (!self.$parentOfOasForOnlineButton.hasClass('disabled')) {
						$.lw_ajax({
							url: args.url,
							success: function(data) {
								if (data && data.Errors && data.Errors.length) {
									self.$parentOfOasForOnlineButton.removeClass('disabled');
									showNotification(null, data.Errors[0].Text, 'error');
								}
							}
						});
					}
					self.$parentOfOasForOnlineButton.addClass('disabled');
				}
			});
		},

		updateOasForOnlineHandler: function(wordCountStatisticsResponse) {
			var self = this;
			self._loadWordcountFields();
			var customerWordCountStatistics = wordCountStatisticsResponse.WordCountStatistics
				? wordCountStatisticsResponse.WordCountStatistics
				: wordCountStatisticsResponse.wordCountStatistics;
			var vendorWordCountStatistics = wordCountStatisticsResponse.VendorWordCountStatistics
				? wordCountStatisticsResponse.VendorWordCountStatistics
				: wordCountStatisticsResponse.vendorWordCountStatistics;

			self.$wordCountTooltip.prop('title', wordCountStatisticsResponse.Status);			
			self.$wordCountTooltip.prop('class', wordCountStatisticsResponse.StatusClass);
			self.$wordCountTooltip.smallipop();

			self.$parentOfOasForOnlineButton.removeClass('disabled');

			self.wordCountStatisticsFields.customer.$wordCount.html(customerWordCountStatistics.WordCount);
			self.wordCountStatisticsFields.customer.$repetitions.html(customerWordCountStatistics.Repetitions);
			self.wordCountStatisticsFields.customer.$noMatch.html(customerWordCountStatistics.NoMatch);
			self.wordCountStatisticsFields.customer.$contextMatch.html(customerWordCountStatistics.ContextMatch);
			self.wordCountStatisticsFields.customer.$percent100.html(customerWordCountStatistics.PercentMatch100);
			self.wordCountStatisticsFields.customer.$percent95to99.html(customerWordCountStatistics.PercentMatch95To99);
			self.wordCountStatisticsFields.customer.$percent85to94.html(customerWordCountStatistics.PercentMatch85To94);
			self.wordCountStatisticsFields.customer.$percent75to84.html(customerWordCountStatistics.PercentMatch75To84);
			self.wordCountStatisticsFields.customer.$percent50to74.html(customerWordCountStatistics.PercentMatch50To74);

			self.wordCountStatisticsFields.vendor.$wordCount.html(vendorWordCountStatistics.WordCount);
			self.wordCountStatisticsFields.vendor.$repetitions.html(vendorWordCountStatistics.Repetitions);
			self.wordCountStatisticsFields.vendor.$noMatch.html(vendorWordCountStatistics.NoMatch);
			self.wordCountStatisticsFields.vendor.$contextMatch.html(vendorWordCountStatistics.ContextMatch);
			self.wordCountStatisticsFields.vendor.$percent100.html(vendorWordCountStatistics.PercentMatch100);
			self.wordCountStatisticsFields.vendor.$percent95to99.html(vendorWordCountStatistics.PercentMatch95To99);
			self.wordCountStatisticsFields.vendor.$percent85to94.html(vendorWordCountStatistics.PercentMatch85To94);
			self.wordCountStatisticsFields.vendor.$percent75to84.html(vendorWordCountStatistics.PercentMatch75To84);
			self.wordCountStatisticsFields.vendor.$percent50to74.html(vendorWordCountStatistics.PercentMatch50To74);
		},

		_initUpdatePageAfterChangeFieldsHandler: function() {
			var self = this;

			self.$sourceFilesTable.on('reloadGrid', function() {
				self._refreshPage();
			});

			self.$referenceFilesTable.on('reloadGrid', function() {
				self._refreshPage();
			});
		},

		_refreshPage: function() {
			$(window).trigger($.Event('navigate'), { RedirectTo: document.URL });
		},

		_initExtractTargetFromSdlxliffHandler: function() {
			var self = this;
			$('#jobDeliveredFilesTable_' + self.options.jobId).on('rowSelected', function(e, args) {
				var $self = $(this);
				var gridSelectedRows = $self.jqGrid('getGridParam', 'selarrrow');
				var containSdlLiffFile = true;
				for (var i = 0; i < gridSelectedRows.length; i++) {
					var rowData = $self.jqGrid('getRowData', gridSelectedRows[i]);
					var fileName = rowData.FileLink;
					if (rowData.FileLink.indexOf('<') > -1) {
						fileName = $(rowData.FileLink).data().titleForSorting;
					}
					if (fileName.split('.').pop() != 'sdlxliff') {
						containSdlLiffFile = false;
					}
				}
				if (containSdlLiffFile) {
					self.$extractTargetFromSdlxliffButton.removeClass('disabled');
				} else {
					self.$extractTargetFromSdlxliffButton.addClass('disabled');
				}
			});
		},

		reloadTargetFilesTable: function() {
			var self = this;
			$('#jobDeliveredFilesTable_' + self.options.jobId).trigger('reloadGrid');
		},

		updateOnlineFlowMessage: function() {
			var self = this;
			var onlineFlowMessageField = self.$elem.find('div[data-field-name="OnlineFlowMessage"] pre')[0];

			$.lw_ajax({
				url: self.getOnlineFlowMessageUrl,
				type: 'GET',
				success: function(data) {
					onlineFlowMessageField.innerText = data;
				}
			});
		}
	};

	// plugin .ctor
	$.fn.job_index = function() {
		return dispatchPluginCall({
			$context: this,
			pluginName: 'job_index',
			pluginObject: pluginObject,
			arguments: arguments,
			defaults: pluginObject.options
		});
	};

})(jQuery);
