if ($env:APPVEYOR_REPO_TAG -eq "true")
{
    Write-Host "Deploying documentation v$($env:APPVEYOR_BUILD_VERSION) to gh-pages ..."
    $gitUrl = "https://github.com/StaMa-StateMachine/StaMa.git"
    $ghPagesGitWorkspacePath = "$($env:APPVEYOR_BUILD_FOLDER)\..\StaMa.gh-pages"
    mkdir $ghPagesGitWorkspacePath
    cd $ghPagesGitWorkspacePath
    git clone $gitUrl .
    git config --global credential.helper store
    Add-Content "$env:USERPROFILE\.git-credentials" "https://$($env:git_gh_pages_access_token):x-oauth-basic@github.com`n"
    git config --global user.name "AppVeyor"
    git config --global user.email "$($env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL)"
    git config --global core.autocrlf false
    git checkout gh-pages
    git rm -rf *
    robocopy /S "$($env:APPVEYOR_BUILD_FOLDER)\bin\netmf\DevelopersGuide" . /fp /ndl
    git add .
    git commit -am "Automated checkin v$($env:APPVEYOR_BUILD_VERSION)"
    git push -f -u origin gh-pages
    Write-Host "Deployed documentation v$($env:APPVEYOR_BUILD_VERSION) to gh-pages."
}
else
{
	Write-Host "Skip gh-pages deployment as environment variable has not matched ("APPVEYOR_REPO_TAG" is "false", should be "true")
}
