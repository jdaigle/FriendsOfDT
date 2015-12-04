<?

function update_peep() {
    checkpass();
    $peep_id = $_REQUEST['peep_id'];
    $hon = mysql_real_escape_string($_REQUEST['hon']);
    $fname = mysql_real_escape_string($_REQUEST['fname']);
    $mname = mysql_real_escape_string($_REQUEST['mname']);
    $nickname = mysql_real_escape_string($_REQUEST['nickname']);
    $lname  = mysql_real_escape_string($_REQUEST['lname']);
    $suffix = mysql_real_escape_string($_REQUEST['suffix']);
    $bio = mysql_real_escape_string($_REQUEST['bio']);
    $email = mysql_real_escape_string($_REQUEST['email']);

    mysql_query ("update people set hon='$hon',fname='$fname',mname='$mname',nickname='$nickname',lname='$lname',suffix='$suffix',bio='$bio',email='$email', last_mod=CURRENT_DATE where ID=$peep_id");
    echo <<< EOT9
    <p align=center><b>Changes Completed!</b><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
    <p align=center>
	<input type='hidden' name='action' value='addpeep'>
	<input type=submit value='Add Another?'>
	</form>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='peep_detail'>
	<input type='hidden' name='peep_id' value='$peep_id'>
    <p align=center>
	<input type=submit value='See the changes'>
	</form>
EOT9;
}

function editpeople($peep_id) {
    include("include/def_pic_choose.php");
    min_auth_level(1,'Not Authorized');
    $peep_select = mysql_query("select hon,fname,mname,nickname,lname,suffix,bio,username,email FROM people WHERE ID=$peep_id");
    while ($pname = mysql_fetch_array($peep_select)) {
	$hon=stripslashes($pname[0]);
	$fname=stripslashes($pname[1]);
	$mname=stripslashes($pname[2]);
	$nickname=stripslashes($pname[3]);
	$lname=stripslashes($pname[4]);
	$suffix=stripslashes($pname[5]);
	$bio=stripslashes($pname[6]);
	$uname=$pname[7];
	$email=stripslashes($pname[8]);
    }
    if (($_SESSION['level'] == 1) && ($_SESSION['username'] != $uname)) {
	echo "<center><font size=+2>Not authorized to edit this record.</font></center>\n";
	endlines();
    }
    echo <<< EOT20
    <table align=center frame=1 width=40%>
    <form method=POST action='$_SERVER[PHP_SELF]'>
    <tr valign=middle><td colspan=2 align=center>
    <b><font size=+2>Add/Edit a Name in IMDT</font></b>
    </td></tr><tr><td colspan=2>&nbsp;</td></tr>
    </td></tr><tr><td colspan=2 align=center>When editing names, please observe the following style rules: Initials should not be followed by a period, nicknames do not require quotes around them (they will be added later).</td></tr>
    </td></tr><tr><td colspan=2>&nbsp;</td></tr>
    <tr><th align=right>Honorific</td><td><input type=text name=hon size=40 value='$hon'></td></tr>
    <tr><th align=right>First Name</th><td><input type=text name=fname size=40 value='$fname'></td></tr>
    <tr><th align=right>Middle Name</td><td><input type=text name=mname size=40 value='$mname'></td></tr>
    <tr><th align=right>Nickname</td><td><input type=text name=nickname size=40 value='$nickname'></td></tr>
    <tr><th align=right>Last Name</td><td><input type=text name=lname size=40 value='$lname'></td></tr>
    <tr><th align=right>Suffix</td><td><input type=text name=suffix size=40 value='$suffix'></td></tr>
    <tr><th align=right>E-mail</td><td><input type=text name=email size=40 value='$email'></td></tr>
    <tr><th align=right>Biographical Data</td><td><textarea name=bio rows=5 cols=30>$bio</textarea></td></tr>
    <tr><td colspan=2>&nbsp;</td></tr>
    <tr align=center valign=middle><td colspan=2 align=center>
    <input type='hidden' name='peep_id' value='$peep_id'>
    <input type='hidden' name='action' value='update_peep'>
    <input type=submit value='Edit Person'>
    </td></tr></form></table><br><br>
EOT20;

    def_pic_choose('people',$peep_id);

#    <table align=center frame=1 width=40%>
#    <tr valign=middle><td colspan=2 align=center>
#    <center><font size=+2>Upload/Update picture in the database</font></td></tr>
#    <tr><td colspan=2 align=center>N.B.: We are only accepting pictures less than 1.5Mb in size, JPEG format with ".jpg" extension.  Anything else will be rejected.</td></tr>
#    <tr valign=middle><td>Current Picture:<br><img src="$thumb"></td>
#    <td align=center><form enctype="multipart/form-data" action="$_SERVER[PHP_SELF]" method="POST">
#      <input type="hidden" name="MAX_FILE_SIZE" value="1000000" />
#      <input type='hidden' name='action' value='update_picture'>
#      <input type='hidden' name='ref_id' value='$peep_id'>
#      <input type='hidden' name='pic_type' value='people'>
#      Choose a file to upload: <input name="uploaded_file" type="file" /><br><br>
#      <input type="submit" value="Upload" />
#    </form></td></tr>
#    </table><br><br>
#EOT20;
}
?>
