<?

function media_header($s_id,$type) {
    include_once ("include/get_fullname.php");
    include_once ("include/get_title.php");
    #Header is fullname if person, title if play
    if ($type == "people") {
	$header=get_fullname($s_id);
	$title_name="<a href=\"?action=peep_detail&amp;peep_id=$s_id\">".$header."</a>";
	echo "<center><font size=+2>Media for $title_name</font><br><br>";
	echo "<a href='?action=media_upload&amp;IDtype=$type&amp;ID=$s_id'>Upload more pictures for this person</a></center><br><br>";
    } elseif ($type == "shows") {
	$header=get_title($s_id);
	$title_name="<a href=\"?action=show_detail&amp;show_id=$s_id\">".$header."</a>";
	echo "<center><font size=+2>Media for $title_name</font><br><br>";
	echo "<a href='?action=media_upload&amp;IDtype=$type&amp;ID=$s_id'>Upload more pictures for this show</a></center><br><br>";
    } elseif ($type == "item") {
	echo "<center><font size=+2>Item $s_id</font><br><br>";
	echo "<a href='?action=media_upload'>Upload a new picture</a></center><br><br>";
    }
}

function media_default() {
    echo <<< EOT
    <font size=+3><center>IMDT Media Center</center></font><br><br>
    <center><b>
	This is the home for headshots, posters, programs, and production stills.  We hope to add movies soon as well.<br>
	If you're logged in, you can <a href="?action=media_upload">Upload a Picture</a> or you can just browse below.
    </b></center><br><br>

    <table align=center border=1 frame=1><tr><td bgcolor='#b6b6f2' colspan=5><font size=+2>Random Files</font></td></tr>\n
    <tr>
EOT;
    $rand10_q= mysql_query("select m.ID,i.thumb from media m,media_items i where m.item_id=i.ID order by RAND() limit 10");
    $counter = 0;
    while ( $rand10_result = mysql_fetch_array($rand10_q)) {
	$ID = $rand10_result[0];
	$thumb = $rand10_result[1];
	echo "<td align=center><a href=\"?action=media_detail&amp;media_id=$ID\"><img src=$thumb></a></td>";
	$counter++;
	if ($counter == 5) {
	    echo "</tr><tr>";
	}
    }
    #show the last 10 files uploaded
    echo "</tr><tr><td bgcolor='#b6b6f2' colspan=5><font size=+2 align=center>Last 10 Files Uploaded</font></td></tr>\n";
    echo "<tr>\n";
    $last10_q= mysql_query("select ID,thumb from media_items order by ID desc limit 10");
    $counter = 0;
    while ( $last10_result = mysql_fetch_array($last10_q)) {
	$thumb = $last10_result[1];
	$ID = $last10_result[0];
	echo "<td align=center><a href=\"?action=media_detail&amp;item_id=$ID\"><img src=$thumb></a></td>";
	$counter++;
	if ($counter == 5) {
	    echo "</tr><tr>";
	}
    }
}

function get_item($item_id) {
    $item_q=mysql_query("select item from media_items where ID=$item_id");
    while ($result = mysql_fetch_array($item_q)) {
	$item=$result[0];
    }
    return($item);
}

function get_thumb($item_id) {
    $item_q=mysql_query("select thumb from media_items where ID=$item_id");
    while ($result = mysql_fetch_array($item_q)) {
	$item=$result[0];
    }
    return($item);
}

function media_index($s_id,$type) {
    if (!isset($s_id)) { media_default(); endlines();}
    media_header($s_id,$type);
    $media_select = mysql_query("select ID,item_id from media where assocID=$s_id and IDtype='$type' order by ID");
    $counter = 0;
    if (mysql_num_rows($media_select) < 1) {echo "<center><h1>No associated media found!<br> Would you like to <a href=\"?action=media_upload&amp;IDtype=$type&amp;ID=$s_id\">upload some</a>?</h1><br><br></center>\n"; endlines();}
    echo "<table align=center border=1><tr>";
    while ($result = mysql_fetch_array($media_select)) {
	$ID=$result[0];
	$picture=get_thumb($result[1]);
	$counter++;
	if ($counter == 4) {
	    echo "</tr><tr>";
	    $counter = 1;
	}
	echo <<< EOT7
	<td align=center valign=middle>
	<div class="rollover">
	<a href="?action=media_detail&amp;media_id=$ID">
	<img src="$picture"></a></div></td>
EOT7;
    }
    echo "</tr></table>";
}

function media_detail($m_id,$i_id) {
    if (isset($m_id)) {
	$detail_select = mysql_query("SELECT assocID,IDtype FROM media where ID=$m_id");
	while ($detail_result = mysql_fetch_array($detail_select)) {
	    $s_id=$detail_result[0];
	    $type=$detail_result[1];
	}

	$order_select = mysql_query("SELECT ID,item_id FROM media where IDtype='".$type."' and assocID=".$s_id." order by ID");
	while ($order_result = mysql_fetch_array($order_select)) {
            if (isset($next)) {
		if ($next == 'next') {
		    $next = $order_result[0];
		}
	    }
            if ($order_result[0] == $m_id && !isset($next)) {
		$i_id = $order_result[1];
        	$next = 'next';
            } elseif (!isset($next)) {
        	$last = $order_result[0];
            }
	}
	if (isset($last)) {
	    $prevlink = "<a href=\"$_SERVER[PHP_SELF]?action=media_detail&amp;media_id=$last\"><--- Previous Item</a>";
	} else {
	    $prevlink = "<--- Previous Item";
	}
	if ($next == 'next') {
	    $nextlink = "Next Item --->";
	} else {
	    $nextlink = "<a href=\"$_SERVER[PHP_SELF]?action=media_detail&amp;media_id=$next\">Next Item ---></a>\n";
	}
    } elseif (isset($i_id)) {
	$s_id=$i_id;
	$type='item';
	$prev_q = mysql_query ("select ID from media_items where ID < $i_id order by ID desc limit 1");
	while ($prev_result =  mysql_fetch_array($prev_q)) { $last=$prev_result[0]; }
	$next_q = mysql_query ("select ID from media_items where ID > $i_id order by ID limit 1");
	while ($next_result =  mysql_fetch_array($next_q)) { $next=$next_result[0]; }
	if (isset($last)) {
	    $prevlink = "<a href=\"$_SERVER[PHP_SELF]?action=media_detail&amp;item_id=$last\"><--- Previous Item</a>";
	} else {
	    $prevlink = "<--- Previous Item";
	}
	if (isset($next)) {
	    $nextlink = "<a href=\"$_SERVER[PHP_SELF]?action=media_detail&amp;item_id=$next\">Next Item ---></a>\n";
	} else {
	    $nextlink = "Next Item --->";
	}
	
    } else { media_default(); endlines();}
    $item=get_item($i_id);
    echo "<table align=center width=100%><tr align=center><td align=left valign=top>$prevlink</td>\n";
    echo "<td align=center>";
    media_header($s_id,$type);
    echo "</td>\n";
    echo "<td align=right valign=top>$nextlink</td></tr></table><br>\n";
    echo "<center><img src='$item'><br><br>";
    echo "This picture is associated with ";
    $i=0;
    $assoc_select = mysql_query("SELECT distinct IDtype,assocID FROM media where item_id=$i_id");
    while ($assoc_result = mysql_fetch_array($assoc_select)) {
	if ($i > 0) {echo ", ";} else {$i=1;}
	if ($assoc_result[0] == "people") {echo "<a href=\"?action=media_index&amp;IDtype=people&amp;ID=$assoc_result[1]\">".get_fullname($assoc_result[1])."</a>\n";}
	if ($assoc_result[0] == "shows") {echo "<a href=\"?action=media_index&amp;IDtype=shows&amp;ID=$assoc_result[1]\">".get_title($assoc_result[1])."</a>\n";}
#	if ($assoc_result[0] == "peep") {echo "<a href=\"?action=peep_detail&amp;peep_id=$assoc_result[1]\">".get_fullname($assoc_result[1])."</a>\n";}
#	if ($assoc_result[0] == "show") {echo "<a href=\"?action=show_detail&amp;show_id=$assoc_result[1]\">".get_title($assoc_result[1])."</a>\n";}
    }
    echo "<br><br>\n";
    echo "See someone in the picture not listed above? Should it be tied to a show? Tie them in!<br>\n";
    if ( $_SESSION['level'] >= 1) {
	echo <<< EOT10
	<table align=center frame=1 width=50%>
        <form method=POST action='$_SERVER[PHP_SELF]'>
        <tr align=center valign=middle><td colspan=2 align=center>
        <font size=+2><b>Tie a Person or Show to Media</b></font>
        </td></tr><tr align=center valign=middle>
        <th align=center>Person</th><th align=center>Show</th></tr>
        <tr><td align=center>
EOT10;
	include_once ("include/peep_option.php");
	peep_option(NULL);
	echo "</td><td align=center>\n";
	include_once ("include/show_option.php");
	show_option(NULL);
	echo <<< EOT10
	</td></tr>
        <tr><td align=center colspan=2>
        <input type='hidden' name='action' value='tie_media'>
        <input type='hidden' name='item_id' value='$i_id'>
EOT10;
	echo <<< EOT10
        <input type=submit value='Tie to Media'>
        </form></td>
EOT10;
	
	echo "<center>";
    } else {
	echo "(You must be logged in to tie people to media.)<br>\n";
    }
}


function tie_media($item_id,$peep_id,$show_id) {
    min_auth_level(1,'Not Authorized');
    # get the item from the DB
    if ($peep_id > 0 ) {
	# verify the person is not already there
	$peep_q=mysql_query("SELECT ID FROM media where assocID=$peep_id and IDtype='people' and item_id=$item_id");
	$peepcheck = mysql_num_rows($peep_q);
	if ($peepcheck == 0 ) {
	# insert row
	    $insert = mysql_query ("insert into media (assocID,IDtype,item_id,last_mod) values ($peep_id,'people',$item_id,CURRENT_DATE)");
    	    if(!$insert){ die("There is little problem: ".mysql_error()); }
	}
    }
    if ($show_id > 0 ) {
	# Verify show is not already there
	$show_q=mysql_query("SELECT ID FROM media where assocID=$show_id and IDtype='shows' and item_id=$item_id");
	$showcheck = mysql_num_rows($show_q);
	if ($showcheck == 0 ) {
	# insert row
	    $insert = mysql_query ("insert into media (assocID,IDtype,item_id,last_mod) values ($show_id,'shows',$item_id,CURRENT_DATE)");
    	    if(!$insert){ die("There is little problem: ".mysql_error()); }

	}
    }
    # redisplay
    media_detail(NULL,$item_id);
}

function media_upload($m_type,$s_id) {
    
#    echo "Feature in Beta.  Try again leta.\n";
#    exit;
    min_auth_level(1,'Sorry, you must be logged in to upload pictures!'); 
    checkpass();
    echo <<< EOT20
    <table align=center width=40%>
    <tr valign=middle><td colspan=2 align=center><form enctype="multipart/form-data" action="$_SERVER[PHP_SELF]" method="POST">
      <input type="hidden" name="MAX_FILE_SIZE" value="1500000" />
      <input type='hidden' name='action' value='update_picture'>
    <center><font size=+2>Upload picture to the Media Center</font></td></tr>
    <tr><td colspan=2 align=center>N.B.: We are currently only accepting pictures less than 1.5Mb in size, JPEG format with ".jpg" extension.  Anything else will be rejected.
    All uploaded pictures must be associated with at least one show or person already in the database.</td></tr>
    <tr><td align=center
EOT20;
    if ($m_type == 'people') { 
	include_once("include/peep_option.php");
	echo " colspan=2>Person: "; 
	peep_option($s_id); 
    } elseif ($m_type == 'shows') { 
	include_once("include/show_option.php");
	echo " colspan=2>Show: "; 
	show_option($s_id); 
    } else { 
	include_once("include/peep_option.php");
	include_once("include/show_option.php");
	echo">Person: "; 
	peep_option(NULL); 
	echo "</td><td align=center>Show: "; 
	show_option(NULL);
    }
    echo "</td></tr>\n";
    echo <<< EOT20
      <tr><td align=center colspan=2>
      Choose a file to upload: <input name="uploaded_file" type="file"><br><br>
      <input type="submit" value="Upload">
    </form></td></tr>
    </table><br><br>
EOT20;

}

function def_pic_change($item_id,$type,$s_id) {
    #Check Permissions First
    $thisguy=$_SESSION['username'];
    $whois_select =  mysql_query ("select ID from people where username='$thisguy'");
    while ($whois_result = mysql_fetch_array($whois_select)) {
	$changing = $whois_result[0];
    }
    # Level 1 can only change their own headshots
#      <input type='hidden' name='ref_id' value='$s_id'>
#      <input type='hidden' name='pic_type' value='$m_type'>
    if (($type=='people') && ($changing == $s_id)) { min_auth_level(1,'Not Authorized'); }
    #Level 2 can change anyone's
    else { min_auth_level(2,'Not Authorized'); }

    $assoc=mysql_query("update $type set media_id=$item_id where ID=$s_id");
    if(!$assoc){ die("There is little problem: ".mysql_error()); }
    if ($type=='people') { $type='peep'; }
    if ($type=='shows') { $type='show'; }
    $action="show_".$type."_detail";
    $to_include="include/".$type."_detail.php";
    include_once("$to_include");
    $action($s_id);
}

?>
