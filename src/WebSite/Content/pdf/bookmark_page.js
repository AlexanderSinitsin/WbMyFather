// bookmark_page.js, ver. 1.0
// visit: www.pdfhacks.com/bookmark_page/

// use this delimiter for serializing our array
var bp_delim= '%#%#';

function SaveData( data ) {
  // data is an array of arrays that needs
  // to be serialized and stored into a persistent
  // global string
  var ds= '';
  for( ii= 0; ii< data.length; ++ii ) {
    for( jj= 0; jj< 3; ++jj ) {
      if( ii!= 0 || jj!= 0 )
        ds+= bp_delim;
      ds+= data[ii][jj];
    }
  }
  global.pdf_hacks_js_bookmarks= ds;
  global.setPersistent( "pdf_hacks_js_bookmarks", true );
}

function GetData() {
  // reverse of SaveData; return an array of arrays
  if( global.pdf_hacks_js_bookmarks== null ) {
    return new Array(0);
  }

  var flat= global.pdf_hacks_js_bookmarks.split( bp_delim );
  var data= new Array();
  for( ii= 0; ii< flat.length; ) {
    var record= new Array();
    for( jj= 0; jj< 3 && ii< flat.length; ++ii, ++jj ) {
      record.push( flat[ii] );
    }
    if( record.length== 3 ) {
      data.push( record );
    }
  }
  return data;
}

function AddBookmark() {
  // query the user for a name, and then combine it with
  // the current PDF page to create a record; store this record
  var label= 
    app.response( "Bookmark Name:",
                  "Bookmark Name",
                  "",
                  false );
  if( label!= null ) {
    var record= new Array(3);
    record[0]= label;
    record[1]= this.path;
    record[2]= this.pageNum;

    data= GetData();
    data.push( record );
    SaveData( data );
  }
}

function ShowBookmarks() {
  // show a pop-up menu; this seems to only work when
  // a PDF is alreay in the viewer;
  var data= GetData();
  var items= '';
  for( ii= 0; ii< data.length; ++ii ) {
    if( ii!= 0 )
      items+= ', ';
    items+= '"'+ ii+ ': '+ data[ii][0]+ '"';
  }
  // assemble the command and the execute it with eval()
  var command= 'app.popUpMenu( '+ items+ ' );';
  var selection= eval( command );
  if( selection== null ) {
    return; // exit
  }

  // the user made a selection; parse out its index and use it
  // to access the bookmark record
  var index= 0;
  // toString() converts the String object to a string literal
  // eval() converts the string literal to a number
  index= eval( selection.substring( 0, selection.indexOf(':') ).toString() );
  if( index< data.length ) {
    try {
      // the document must be 'disclosed' for us to have any access
      // to its properties, so we use these FirstPage NextPage calls
      //
      app.openDoc( data[index][1] );
      app.execMenuItem( "FirstPage" );
      for( ii= 0; ii< data[index][2]; ++ii ) {
        app.execMenuItem( "NextPage" );
      }
    }
    catch( ee ) {
      var response= 
        app.alert("Error trying to open the requested document.\nShould I remove this bookmark?", 2, 2);
      if( response== 4 && index< data.length ) {
        data.splice( index, 1 );
        SaveData( data );
      }
    }
  }
}

function DropBookmark() {
  // modelled after ShowBookmarks()
  var data= GetData();
  var items= '';
  for( ii= 0; ii< data.length; ++ii ) {
    if( ii!= 0 )
      items+= ', ';
    items+= '"'+ ii+ ': '+ data[ii][0]+ '"';
  }
  var command= 'app.popUpMenu( '+ items+ ' );';
  var selection= eval( command );
  if( selection== null ) {
    return; // exit
  }

  var index= 0;
  index= eval( selection.substring( 0, selection.indexOf(':') ).toString() );
  if( index< data.length ) {
    data.splice( index, 1 );
    SaveData( data );
  }
}

function ClearBookmarks() {
  if( app.alert("Are you sure you want to erase all bookmarks?", 2, 2 )== 4 ) {
    SaveData( new Array(0) );
  }
}

app.addMenuItem( {
cName: "-",              // menu divider
cParent: "View",         // append to the View menu
cExec: "void(0);" } );

app.addMenuItem( {
cName: "Bookmark This Page &5",
cParent: "View",
cExec: "AddBookmark();",
cEnable: "event.rc= (event.target != null);" } );

app.addMenuItem( {
cName: "Go To Bookmark &6",
cParent: "View",
cExec: "ShowBookmarks();",
cEnable: "event.rc= (event.target != null);" } );

app.addMenuItem( {
cName: "Remove a Bookmark",
cParent: "View",
cExec: "DropBookmark();",
cEnable: "event.rc= (event.target != null);" } );

app.addMenuItem( {
cName: "Clear Bookmarks",
cParent: "View",
cExec: "ClearBookmarks();",
cEnable: "event.rc= true;" } );