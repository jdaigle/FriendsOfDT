<?

function editshows($show_id) {
    min_auth_level(2,'Not Authorized');
    include_once ("include/q_option.php");
    include_once ("include/def_pic_choose.php");
    $show_select = mysql_query("select title,quarter,author,year,pictures,funfacts,toaster FROM shows WHERE ID=$show_id");
    while ($sname = mysql_fetch_array($show_select)) {
	$title=stripslashes($sname[0]);
	$quarter=$sname[1];
	$author=stripslashes($sname[2]);
	$year=stripslashes($sname[3]);
	$pictures=stripslashes($sname[4]);
	$funfacts=stripslashes($sname[5]);
	$toaster=stripslashes($sname[6]);
    }
    echo <<< EOT20
    <center> Conventions: If a title starts with "A", "An", or "The", please put the article at the end of the title following a comma and a space.  This will aid in proper alphabetization.<br><br></center>
    <form method=POST action='$_SERVER[PHP_SELF]'>
    <table align=center frame=1 width=40%>
    <tr valign=middle><td colspan=2 align=center>
    <b><font size=+2>Add/Edit a Show in IMDT</font></b>
    </td></tr><tr><td colspan=2>&nbsp;</td></tr>
    <tr><th align=right>Show Title</th><td><input type=text name=title size=40 value="$title"></td></tr>
    <tr><th align=right>Author</th><td><input type=text name=author size=40 value="$author"></td></tr>
    <tr><th align=right>Quarter/Semester</th><td><select name='quarter'>
EOT20;
    q_option($quarter);
    echo <<< EOT20
    </select></td></tr>
    <tr><th align=right>Year</th><td><input type=text name=year size=40 value='$year'></td></tr>
    <tr><th align=right>Link to Pictures</th><td><input type=text name=pictures size=40 value='$pictures'></td></tr>
    <tr><th align=right>Fun Facts</th><td><textarea name=funfacts rows=5 cols=30>$funfacts</textarea></td></tr>
    <tr><th align=right>Where Was The Toaster?</th><td><textarea name=toaster rows=5 cols=30>$toaster</textarea></td></tr>
    <tr><td colspan=2>&nbsp;</td></tr>
    <tr align=center valign=middle><td colspan=2 align=center>
    <input type='hidden' name='show_id' value='$show_id'>
    <input type='hidden' name='action' value='update_show'>
    <input type=submit value='Edit Show'>
    </td></tr></table><br><br>
    </form>
EOT20;
    def_pic_choose('shows',$show_id);

#    <table align=center frame=1 width=40%>
#    <tr valign=middle><td colspan=2 align=center>
#    <center><font size=+2>Upload/Update poster in the database</font></center></td></tr>
#    <tr><td colspan=2 align=center>N.B.: We are only accepting files less than 1.5Mb in size, JPEG format with ".jpg" extension.  Anything else will be rejected.</td></tr>
#    <tr valign=middle><td>Current Picture:<br><img src="$thumb"></td>
#    <td align=center><form enctype="multipart/form-data" action="$_SERVER[PHP_SELF]" method="POST">
#      <input type="hidden" name="MAX_FILE_SIZE" value="1000000" />
#      <input type='hidden' name='action' value='update_picture'>
#      <input type='hidden' name='ref_id' value='$show_id'>
#      <input type='hidden' name='pic_type' value='shows'>
#      Choose a file to upload: <input name="uploaded_file" type="file" /><br><br>
#      <input type="submit" value="Upload" />
#    </form></td></tr>
#    </table><br><br>
#EOT20;
}

function update_show($show_id) {
    checkpass();
    $show_id = mysql_real_escape_string($_REQUEST['show_id']);
    $title = mysql_real_escape_string($_REQUEST['title']);
    $quarter = $_REQUEST['quarter'];
    check_number($quarter);
    $author = mysql_real_escape_string($_REQUEST['author']);
    $year = $_REQUEST['year'];
    check_number($year);
    $pictures = mysql_real_escape_string($_REQUEST['pictures']);
    $funfacts = mysql_real_escape_string($_REQUEST['funfacts']);
    $toaster = mysql_real_escape_string($_REQUEST['toaster']);
    mysql_query ("update shows set title='$title',quarter='$quarter',author='$author',year='$year',pictures='$pictures',funfacts='$funfacts',toaster='$toaster',last_mod=CURRENT_DATE where ID=$show_id");
    include_once ("include/show_detail.php");
    show_show_detail($show_id);
}
?>
