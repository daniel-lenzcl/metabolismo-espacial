mergeInto(LibraryManager.library, {
UploadFile: function (gameObjectName, methodName, filter) {
        var nome_objeto = UTF8ToString(gameObjectName);
        var nome_metodo =  UTF8ToString(methodName);
        var nome_filter =  UTF8ToString(filter);        //funciona o utf fora da function reader.onload, mas usar o utf la dentro nao funciona
        var input = document.createElement("input");
        input.type = "file";
        input.accept = UTF8ToString(filter); // Exemplo: ".obj"
        input.onchange = function(event) {
            var file = event.target.files[0];
            var reader = new FileReader();
            reader.onload = function() {
                var data = reader.result; // data:...;base64,...
//                console.log('console msg', file.name, file.size);
                SendMessage(nome_objeto, nome_metodo, data);
            };
            reader.readAsDataURL(file);
        };
        input.click();
},
logMessage: function (message) {
        var msg = UTF8ToString(message);
        console.log(msg);
},
 download_rede: function(filenamePtr, textPtr) {
    // Converte ponteiros UTF8 para strings JavaScript
    var filename = UTF8ToString(filenamePtr || 0) || "rede.csv";
    var text = UTF8ToString(textPtr || 0) || "";

    try {
      // Preferível usar Blob (mais robusto para strings grandes)
      var blob = new Blob([text], { type: 'text/csv;charset=utf-8;' });
      var url = URL.createObjectURL(blob);
      var a = document.createElement('a');
      a.href = url;
      a.download = filename;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
      console.log('download_rede: download iniciado', filename, 'tamanho:', text.length);
    } catch (e) {
      // Fallback simples usando data URI — menos ideal para textos grandes
      try {
        var element = document.createElement('a');
        element.setAttribute('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(text));
        element.setAttribute('download', filename);
        element.style.display = 'none';
        document.body.appendChild(element);
        element.click();
        document.body.removeChild(element);
        console.warn('download_rede: fallback via data URI usado', e);
      } catch (err) {
        console.error('download_rede: falha ao gerar download', err);
      }
    }
  }
});
