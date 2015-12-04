<?
function def_pic_choose($pic_type,$s_id) {
    $counter = 0;
    $head_query = mysql_query("select media_id from $pic_type where ID=$s_id");
    while ($result = mysql_fetch_array($head_query)) {
	$def_pic_id = $result[0];
    }

    echo <<< EOT
    <table align=center frame=1 width=60%>
    <tr valign=middle><td align=center>
    <font size=+2>Set default picture for this record</font><br>
    If you do not see a picture you want to associate, please 
    <a href="?action=media_upload&amp;IDtype=$pic_type&amp;ID=$s_id">upload one!</a>
    </td></tr>
    <tr><td align=center>
      <table align=center width =100%><tr><form enctype="multipart/form-data" action="$_SERVER[PHP_SELF]" method="POST">
      <input type='hidden' name='action' value='def_pic_change'>
      <input type='hidden' name='ID' value=$s_id>
      <input type='hidden' name='IDtype' value='$pic_type'>
EOT;
    $assoc_query =  mysql_query("select ID from media where IDtype='$pic_type' and assocID=$s_id");
    $assoc_num = mysql_num_rows($assoc_query);
    if ($assoc_num == 0) {
	#User has no associated media
	echo "<td align=center><font size=+2>No media associated!</font><br>\n";
	echo "Before you can choose a default picture, one must be uploaded to the media center and associated with this record.<br>\n";
	echo "You can do that at the <a href=\"?action=media_upload&amp;type=$pic_type&amp;ID=$s_id\">Media Upload Page</a>\n";
	echo "</td></tr></table></table>\n";
	endlines();
    }
    include_once("include/get_pic.php");
    while ($assoc_result = mysql_fetch_array($assoc_query)) {
	$new_m_id = $assoc_result[0];
	list ($tiny,$thumb) = get_tinypic_from_media($pic_type,$assoc_result[0]);
        $counter++;
	if ($counter == 8) {
	    $high_counter = $counter;
	    echo "</tr><tr>\n";
	    $counter = 1;
	}
#	echo "<td><center><img src=\"$tiny\"><br><input type='radio' name='media_id' value=$new_m_id";
	echo "<td><center><a class='thumbnail' href='#thumb'><img src=\"$tiny\" border='0' /><span><img src=\"$thumb\" /><br /></span></a><br>";
	echo "<input type='radio' name='media_id' value=$new_m_id";
	if ($new_m_id == $def_pic_id) { echo " checked"; }
	echo "></center></td>\n";
    }
    if (! isset($high_counter)) {$high_counter = $counter;}
    echo <<< EOT
    </tr>
    <tr><td colspan=$high_counter align=center><input type="submit" value="Change Default Picture" />
    </form></table></td></tr>
    </table><br><br>
EOT;

}

?>
