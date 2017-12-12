(function($){
 
 //初始化清除按钮
 function initClear(target){
  var jq = $(target);
  var opts = jq.data('combo').options;
  var combo = jq.data('combo').combo;
  var arrow = combo.find('span.textbox-addon');
  
  var clear = arrow.siblings("span.combo-clear");
  if(clear.size()==0){
   //创建清除按钮。
   clear = $('<span class="combo-clear" style="height: 20px;"></span>');
   
   //清除按钮添加悬停效果。
   clear.unbind("mouseenter.combo mouseleave.combo").bind("mouseenter.combo mouseleave.combo",
    function(event){
     var isEnter = event.type=="mouseenter";
     clear[isEnter ? 'addClass' : 'removeClass']("combo-clear-hover");
    }
   );
   //清除按钮添加点击事件，清除当前选中值及隐藏选择面板。
   clear.unbind("click.combo").bind("click.combo",function(){
    jq.combo("setValue","").combo("setText","");
    jq.combo('hidePanel');
   });
   arrow.before(clear);
  };
  var input = combo.find("input.textbox-text");
  input.outerWidth(input.outerWidth()-clear.outerWidth());
  
  opts.initClear = true;//已进行清除按钮初始化。
 }
 

}(jQuery));
