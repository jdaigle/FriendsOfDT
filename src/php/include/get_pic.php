<?

function get_thumb_link($type,$ID) {
    $med_select = mysql_query("select media_id from $type where ID=$ID");
    while ($med_result = mysql_fetch_array($med_select)) {
	$media_id=$med_result[0];
	if  ($type == 'people') {
	    include_once ("include/get_fullname.php");
	    $wholename = get_fullname($ID);
	} elseif ($type == 'shows') {
	    include_once ("include/get_title.php");
	    $wholename = get_title($ID);
	}
#	$pic_select = mysql_query("select thumb,item from media_items where ID=(select item_id from media where ID=$media_id)");
	$pic_select = mysql_query("select thumb from media_items where ID=(select item_id from media where ID=$media_id)");
	while ($result = mysql_fetch_array($pic_select)) {
	    $thumb = $result[0];
#	    $picture = $result[1];
	}
	if ($media_id == 1) {
	    $image_link = "<img src='$thumb'>";
	}else{
#	    $image_link="<a target='new' href='$picture'><img src='$thumb' alt=\"$wholename\"></a>";
	    $image_link="<a href='?action=media_detail&media_id=$media_id'><img src='$thumb' alt=\"$wholename\"></a>";
	}
    }
    return ($image_link);
}

function get_tinypic_from_media($type,$m_ID) {
    $tiny_select = mysql_query("select tiny,thumb from media_items where ID=(select item_id from media where ID=$m_ID)");
    while ($result = mysql_fetch_array($tiny_select)) { $tinypic = $result[0]; $thumb=$result[1]; }
    return array ($tinypic,$thumb);
}

function get_tinypic($type,$p_ID) {
    if ($type == 'peep') { $type = 'people'; }
    if ($type == 'show') { $type = 'shows'; }
    $tiny_select = mysql_query("select tiny from media_items where ID=(select item_id from media where ID=(select media_id from $type where ID=$p_ID))");
    while ($result = mysql_fetch_array($tiny_select)) { $tinypic = $result[0]; }
    return ($tinypic);
}
?>
