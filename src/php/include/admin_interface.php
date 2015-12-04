<?

include_once ("include/EC_option.php");
include_once ("include/awards_option.php");
include_once ("include/job_option.php");
include_once ("include/show_option.php");
include_once ("include/peep_option.php");

function admin_framework() {
    min_auth_level(4,'Not Authorized');
    echo <<< EOT10
	<table rowsep=1 frame=1 align=center width=50%>
	<tr align=center valign=middle><td align=right>
	<b>Add a Person to the database</b>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addpeep'>
	<input type=submit value='Add New Person'>
	</form>
	</td></tr>

	<tr align=center valign=middle><td align=right>
	<b>Add a Crew Position to the database</b>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addjobs'>
	<input type=submit value='Add New Job'>
	</form>
	</td></tr>

	<tr align=center valign=middle><td align=right>
	<b>Add a Show to the database</b>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addshow'>
	<input type=submit value='Add New Show'>
	</form>
	</td></tr>

	<tr align=center valign=middle><td align=right>
	<b>Add a New Position to the EC</b>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addEC_list'>
	<input type=submit value='Add EC Position'>
	</form>
	</td></tr>

	<tr align=center valign=middle><td align=right>
	<b>Add a Person to the EC</b>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addEC'>
	<input type=submit value='Add EC Member'>
	</form>
	</td></tr>

	<tr align=center valign=middle><td align=right>
	<b>Add a New Award</b>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addawards_list'>
	<input type=submit value='Add New Award'>
	</form>
	</td></tr>

	<tr align=center valign=middle><td align=right>
	<b>Add an Award Recipient</b>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addawards'>
	<input type=submit value='Add Award Recipient'>
	</form>
	</td></tr></table>

	<table align=center frame=1 width=50%>
	<tr align=center valign=middle><td align=right>
	<b>Edit a Person</b>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
EOT10;

    peep_option(NULL);
    echo <<< EOT10
	<input type='hidden' name='action' value='editpeep'>
	<input type=submit value='Edit Person'>
	</form>
	</td></tr>

	<tr border=1 align=center valign=middle><td align=right>
	<b>Edit a Job</b><p>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='editjobs'>
EOT10;
    job_option(NULL);
    echo <<< EOT20
	<input type=submit value='Edit Job'>
	</form>
	</td></tr>
	</table>

	<table align=center frame=1 width=50%>
	<tr align=center valign=middle><td align=right>
	<b>Edit a Show</b><p>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='editshow'>
EOT20;
    show_option(NULL);
    echo <<< EOT20
	<br>
	<input type=submit value='Edit Show Details'>
	</form>
	</td></tr>
	</table>

	<table align=center frame=1 width=50%>
	<tr align=center valign=middle><td align=right>
	<b>Add Cast to a Show</b><p>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addcast'>
EOT20;
    show_option(NULL);
    echo <<< EOT20
	<br>
	<input type=submit value='Add Cast to Show'>
	</form>
	</td></tr>
	</table>

	<table align=center frame=1 width=50%>
	<tr align=center valign=middle><td align=right>
	<b>Add Crew to a Show</b><p>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addcrew'>
EOT20;
    show_option(NULL);
    echo <<< EOT20
	</td></tr><tr><td colspan=2 align=center><input type=submit value='Add Crew to Show'>
	</form>
	</td></tr>
	</table>

	<table align=center frame=1 width=50%>
	<tr align=center valign=middle><td align=right>
	<b>Edit the Name of an Award</b><p>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='editawards_list'>
EOT20;
    awards_option(NULL);
    echo <<< EOT20
	</td></tr><tr><td colspan=2 align=center><input type=submit value='Edit Award Name'>
	</form>
	</td></tr>
	</table>

	<table align=center frame=1 width=50%>
	<tr align=center valign=middle><td align=right>
	<b>Edit the Title of an EC Position</b><p>
	</td><td>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='editEC_list'>
EOT20;
    EC_option(NULL);
    echo <<< EOT20
	</td></tr><tr><td colspan=2 align=center><input type=submit value='Edit EC Title'>
	</td></tr>
	</form></table>
EOT20;
    min_auth_level(5,'');
    echo <<< EOT20
	<table align=center frame=1 width=50%>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<tr align=center valign=middle><td align=right>
	<b>Approve New Members: </b>
	</td>
	<input type='hidden' name='action' value='approval_page'>
	<td align=center><input type=submit value='Approve New Members'>
	</td></tr></form>

	<form method=GET action='$_SERVER[PHP_SELF]'>
	<tr align=center valign=middle><td align=right>
	<b>Modify Member Access Levels: </b>
	</td>
	<input type='hidden' name='action' value='chlevel_page'>
	<td align=center><input type=submit value='Modify Access Levels'>
	</td></tr><table></form>
EOT20;
}
?>
