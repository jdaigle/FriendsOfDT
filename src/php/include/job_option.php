<?

function job_option($job_id,$s_crit) {
    echo <<< EOT10
	<input type='hidden' name='action' value='editjobs'>
	<select name='job_id'>
	<option value=''></option>
EOT10;
    if ((isset($s_crit)) && ($s_crit == 'pri')) { $sort = "priority"; } else { $sort = "job"; }
    $job_select = mysql_query("SELECT ID,job FROM jobs where job is not null ORDER by $sort");
    while ($jobs = mysql_fetch_array($job_select)) {
	$title=stripslashes($jobs[1]);
	echo "	<option value='$jobs[0]'";
	if (isset($job_id) && ($jobs[0] == $job_id)) { echo " selected"; }
	echo ">$title</option>\n";
    }
    echo "</select>\n";

}

?>
