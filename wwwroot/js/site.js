document.addEventListener('DOMContentLoaded', function () {
  // 1. Auto-dismiss success/warning alerts after 6 seconds.
  document.querySelectorAll('.auto-dismiss').forEach(function (el) {
    setTimeout(function () {
      if (window.bootstrap) { bootstrap.Alert.getOrCreateInstance(el).close(); }
    }, 6000);
  });

  // 2. Registration form: show the fee of the selected event.
  var eventSelect = document.getElementById('eventSelect');
  var feePreview = document.getElementById('feePreview');
  function updateFee() {
    if (!eventSelect || !feePreview) { return; }
    var opt = eventSelect.options[eventSelect.selectedIndex];
    if (!opt || !opt.dataset.label) { feePreview.classList.add('d-none'); return; }
    feePreview.innerHTML = '<i class="bi bi-receipt"></i> Registration fee: <strong>' +
      opt.dataset.label + '</strong> (' + opt.dataset.category + ')';
    feePreview.classList.remove('d-none');
  }
  if (eventSelect) { eventSelect.addEventListener('change', updateFee); updateFee(); }

  // 3. Dashboard: live search over the participant table.
  var search = document.getElementById('tableSearch');
  if (search) {
    search.addEventListener('input', function () {
      var q = search.value.toLowerCase();
      document.querySelectorAll('#participantTable tbody tr').forEach(function (row) {
        row.style.display = row.textContent.toLowerCase().indexOf(q) > -1 ? '' : 'none';
      });
    });
  }
});
