(function () {
  const tokenKey = 'swagger_token';

  function setAuthHeaderForFetch() {
    const originalFetch = window.fetch;
    window.fetch = function (input, init = {}) {
      try {
        const token = localStorage.getItem(tokenKey);
        if (token) {
          init.headers = init.headers || {};
          if (init.headers instanceof Headers) {
            init.headers.set('Authorization', 'Bearer ' + token);
          } else {
            init.headers['Authorization'] = 'Bearer ' + token;
          }
        }
      } catch (e) { }
      return originalFetch(input, init);
    };
  }

  function setAuthHeaderForXhr() {
    const origOpen = XMLHttpRequest.prototype.open;
    const origSend = XMLHttpRequest.prototype.send;

    XMLHttpRequest.prototype.open = function () {
      this._opened = true;
      return origOpen.apply(this, arguments);
    };

    XMLHttpRequest.prototype.send = function () {
      try {
        const token = localStorage.getItem(tokenKey);
        if (token) {
          this.setRequestHeader('Authorization', 'Bearer ' + token);
        }
      } catch (e) { }
      return origSend.apply(this, arguments);
    };
  }

  function addUI() {
    const container = document.createElement('div');
    container.style = 'padding:8px;display:flex;gap:8px;align-items:center;';

    const input = document.createElement('input');
    input.type = 'text';
    input.placeholder = 'Bearer token';
    input.style = 'flex:1;padding:6px;border:1px solid #ddd;border-radius:4px;';
    const btnSet = document.createElement('button');
    btnSet.innerText = 'Set token';
    btnSet.style = 'padding:6px 8px;';
    btnSet.onclick = function () {
      localStorage.setItem(tokenKey, input.value);
      alert('Token saved for Swagger requests');
    };

    const btnClear = document.createElement('button');
    btnClear.innerText = 'Clear';
    btnClear.style = 'padding:6px 8px;';
    btnClear.onclick = function () {
      localStorage.removeItem(tokenKey);
      input.value = '';
      alert('Token cleared');
    };

    container.appendChild(input);
    container.appendChild(btnSet);
    container.appendChild(btnClear);

    // Hide by default; will be toggled by the topbar Authorize button
    container.style.display = 'none';

    // Insert container below topbar if available
    function insert() {
      const topbar = document.querySelector('.swagger-ui .topbar');
      if (topbar && topbar.parentNode) {
        topbar.parentNode.insertBefore(container, topbar.nextSibling);
        return true;
      }
      return false;
    }

    // Try multiple times until Swagger UI is ready
    let attempts = 0;
    const iv = setInterval(() => {
      if (insert() || attempts++ > 20) {
        // Also add an Authorize button to the topbar
        const topbar = document.querySelector('.swagger-ui .topbar');
        if (topbar) {
          // Authorize button
          if (!document.getElementById('custom-authorize-btn')) {
            const authBtn = document.createElement('button');
            authBtn.id = 'custom-authorize-btn';
            authBtn.innerText = 'Authorize';
            authBtn.style = 'margin-left:8px;padding:6px 10px;background:#1976d2;color:#fff;border:none;border-radius:4px;cursor:pointer;';
            authBtn.onclick = function () {
              // Toggle visibility of the token container
              container.style.display = container.style.display === 'none' ? 'flex' : 'none';
            };
            topbar.appendChild(authBtn);
          }

          // Clear token button in topbar
          if (!document.getElementById('custom-clear-btn')) {
            const clearBtn = document.createElement('button');
            clearBtn.id = 'custom-clear-btn';
            clearBtn.innerText = 'Clear Token';
            clearBtn.style = 'margin-left:8px;padding:6px 10px;background:#e53935;color:#fff;border:none;border-radius:4px;cursor:pointer;';
            clearBtn.onclick = function () {
              localStorage.removeItem(tokenKey);
              alert('Token cleared');
            };
            topbar.appendChild(clearBtn);
          }
        }
        clearInterval(iv);
      }
    }, 500);
  }

  try {
    setAuthHeaderForFetch();
    setAuthHeaderForXhr();
    addUI();
  } catch (e) {
    // ignore
  }
})();
