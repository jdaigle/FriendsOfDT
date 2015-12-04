<?

function editjobs($edit_job_id) {
    min_auth_level(4,'Not Authorized');
    check_number($edit_job_id);
    $job_select = mysql_query("select job,URL FROM jobs WHERE ID=$edit_job_id");
    while ($jname = mysql_fetch_array($job_select)) {
	$job=stripslashes($jname[0]);
	$URL=stripslashes($jname[1]);
    }
    echo <<< EOT20
    <table align=center frame=1 width=40%>
    <form method=POST action='$_SERVER[PHP_SELF]'>
    <tr valign=middle><td colspan=3 align=center><b><font size=+2>Add/Edit a Job in IMDT</font></b></td></tr>
    <tr><td colspan=3><br>Be aware!  This will affect all records associated with this job title.  These have been made into a fixed table to avoid minor variations and typos.  Do not change/add a job lightly!<br><br>
    Also, jobs are displayed in a hierarchical priority.  Please select the existing job that this listing should be displayed above.  If you are merely editing a name and not changing priority ordering, just leave it as it stands.<br><br></td></tr>
    <tr><th align=center>Job Title</th><th align=center>Display above</th><th align=center>Glossary URL</th></tr>
    <tr><td align=center><input type=text name=job size=40 value='$job'></td>
    <td>
EOT20;
    include_once ("include/job_option.php");
    job_option($edit_job_id,'pri');
    echo <<< EOT20
    </td><td align=center><input type=text name=URL size=50 value="$URL"></td>
    </tr>
    <tr><td colspan=2>&nbsp;</td></tr>
    <tr align=center valign=middle><td colspan=3 align=center>
    <input type='hidden' name='edit_job_id' value='$edit_job_id'>
    <input type='hidden' name='action' value='update_job'>
    <input type=submit value='Edit Job'>
    </td></tr></form></table><br><br>
EOT20;
}

function update_job($job_id,$new_pri_id) {
    checkpass();
    $job = mysql_real_escape_string($_REQUEST['job']);
    $URL = mysql_real_escape_string($_REQUEST['URL']);
    $pri_query = mysql_query("select priority from jobs where ID=$job_id");
    while ($pri_result = mysql_fetch_array($pri_query)) { $cur_pri = $pri_result[0]; }
    if ($cur_pri < 1) {
	$max_id_query = mysql_query ("select max(priority) from jobs");
	while ($max_pri_result = mysql_fetch_array($max_id_query)) {
	    $max_pri = $max_pri_result[0];
	}
	$cur_pri = $max_pri + 1;
    }
    $new_pri_query = mysql_query("select priority from jobs where ID=$new_pri_id");
    while ($new_pri_result = mysql_fetch_array($new_pri_query)) { $new_pri = $new_pri_result[0]; }
    if ($cur_pri == $new_pri) {
	mysql_query ("update jobs set job='$job', URL='$URL' where ID=$job_id");
    } else {
	if ($cur_pri < $new_pri) { 
	    $new_pri = $new_pri - 1;
	    mysql_query ("update jobs set priority = priority - 1 where priority >= $cur_pri and priority <= $new_pri");
	} else { 
	    mysql_query ("update jobs set priority = priority + 1 where priority >= $new_pri and priority <= $cur_pri");
	}
	mysql_query ("update jobs set job='$job', priority=$new_pri, URL='$URL' where ID=$job_id");
    }
    echo "<p align=center><b>Changes Completed!</b><br>\n";
}
?>
